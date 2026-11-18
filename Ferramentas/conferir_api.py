#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Confere o ClienteApi.cs contra o CODIGO da API, nao contra a memoria.

POR QUE ISTO EXISTE
O cliente HTTP do desktop e do site precisa acertar duas coisas que o
compilador do C# nao tem como verificar:

  1. os caminhos das rotas  ("/api/cursos/:id")
  2. os nomes dos campos do JSON  ("message", e nao "mensagem")

Errar qualquer uma compila, roda e falha so quando alguem clica. Este
script le o server.js e os routers montados, le o ClienteApi.cs, e
compara os dois.

COMO RODAR
    python3 Ferramentas/conferir_api.py <pasta-da-api> [ClienteApi.cs]

    <pasta-da-api> e a pasta "api/src" do branch main.

QUANDO RODAR
Toda vez que a API mudar de rota ou de nome de campo.
"""

import os
import re
import sys


def carregar_rotas_da_api(pasta_api):
    """Rotas que a API realmente expoe.

    Conta so o que esta MONTADO: ha arquivos em routes/ que nao sao
    carregados pelo server.js e cujas rotas nao respondem.
    """
    caminho_server = os.path.join(pasta_api, 'server.js')
    server = open(caminho_server, encoding='utf-8').read()

    rotas = set()

    # rotas escritas direto no server.js
    for m in re.finditer(r"app\.(get|post|put|delete)\(\s*'([^']+)'", server):
        rotas.add((m.group(1).upper(), m.group(2)))

    # routers montados com app.use('/prefixo', require('./routes/x'))
    for m in re.finditer(r"app\.use\(\s*'([^']+)'\s*,\s*require\('\./routes/(\w+)'\)",
                         server):
        prefixo, arquivo = m.group(1), m.group(2)
        caminho = os.path.join(pasta_api, 'routes', arquivo + '.js')

        if not os.path.exists(caminho):
            continue

        fonte = open(caminho, encoding='utf-8').read()

        for r in re.finditer(r"router\.(get|post|put|delete)\(\s*'([^']+)'", fonte):
            sufixo = '' if r.group(2) == '/' else r.group(2)
            rotas.add((r.group(1).upper(), prefixo + sufixo))

    return rotas


def carregar_campos_da_api(pasta_api):
    """Nomes de campo que a API produz.

    Vem de duas fontes: os apelidos do SQL ("c.Titulo AS titulo") e as
    chaves dos objetos devolvidos por res.json.
    """
    campos = set()

    for pasta, _, nomes in os.walk(pasta_api):
        for nome in nomes:
            if not nome.endswith('.js'):
                continue

            fonte = open(os.path.join(pasta, nome), encoding='utf-8').read()

            campos |= set(re.findall(r'\bAS\s+(\w+)', fonte, re.IGNORECASE))
            campos |= set(re.findall(r'(\w+)\s*:', fonte))

    return {c.lower() for c in campos}


def padronizar(rota):
    """Deixa a rota num formato comparavel: :id e valores viram <>."""
    rota = rota.split('?')[0].rstrip('/')
    return re.sub(r':[A-Za-z]\w*', '<>', rota)


def carregar_rotas_do_cliente(caminho_cliente):
    """Rotas que o ClienteApi.cs chama."""
    cliente = open(caminho_cliente, encoding='utf-8-sig').read()

    # rotas montadas em variavel antes da chamada
    variaveis = {}
    for m in re.finditer(r'string\s+(afr_\w+)\s*=\s*((?:[^;]|\n)+?);', cliente):
        variaveis[m.group(1)] = m.group(2)

    def expressao_para_rota(expr):
        partes = []

        for pedaco in re.split(r'\+', expr):
            pedaco = pedaco.strip()
            literal = re.fullmatch(r'"([^"]*)"', pedaco)

            if literal:
                partes.append(literal.group(1))
            elif pedaco.startswith('Uri.EscapeDataString'):
                partes.append('')          # querystring
            else:
                partes.append('<>')        # um valor qualquer

        return padronizar(''.join(partes))

    rotas = set()

    for m in re.finditer(
            r'Chamar\(HttpMethod\.(\w+),\s*((?:[^;]|\n)+?),\s*(?:null|afr_corpo)\)',
            cliente):
        expr = m.group(2).strip()
        expr = variaveis.get(expr, expr)
        rotas.add((m.group(1).upper(), expressao_para_rota(expr)))

    campos = set(re.findall(r'JsonPropertyName\("(\w+)"\)', cliente))

    return rotas, campos


def main():
    if len(sys.argv) < 2:
        print(__doc__)
        return 2

    pasta_api = sys.argv[1]

    caminho_cliente = (sys.argv[2] if len(sys.argv) > 2
                       else 'CSharp/PlataformaCursos/ClienteApi.cs')

    if not os.path.isdir(pasta_api):
        print(f'Pasta da API nao encontrada: {pasta_api}')
        return 2

    if not os.path.exists(caminho_cliente):
        print(f'ClienteApi nao encontrado: {caminho_cliente}')
        return 2

    rotas_api = carregar_rotas_da_api(pasta_api)
    formatos_api = {(m, padronizar(r)) for m, r in rotas_api}
    campos_api = carregar_campos_da_api(pasta_api)

    rotas_cliente, campos_cliente = carregar_rotas_do_cliente(caminho_cliente)

    problemas = []

    print('=== ROTAS ===')
    for metodo, rota in sorted(rotas_cliente):
        ok = (metodo, rota) in formatos_api
        print(f'  [{"ok  " if ok else "ERRO"}] {metodo:6} {rota or "(vazia)"}')
        if not ok:
            problemas.append(f'rota {metodo} {rota} nao existe na API')

    print()
    print('=== NOMES DE CAMPO ===')
    for campo in sorted(campos_cliente):
        ok = campo.lower() in campos_api
        print(f'  [{"ok  " if ok else "ERRO"}] {campo}')
        if not ok:
            problemas.append(f'campo "{campo}" nao aparece na API')

    print()
    print(f'{len(rotas_cliente)} rotas chamadas, {len(formatos_api)} expostas pela API.')

    nao_usadas = sorted(formatos_api - rotas_cliente)
    if nao_usadas:
        print()
        print('Rotas da API ainda nao usadas pelo C# (so informativo):')
        for metodo, rota in nao_usadas:
            print(f'  {metodo:6} {rota}')

    print()
    if problemas:
        print(f'PROBLEMAS ({len(problemas)}):')
        for p in problemas:
            print('  -', p)
        return 1

    print('Tudo confere com o codigo da API.')
    return 0


if __name__ == '__main__':
    sys.exit(main())
