#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Validador estatico do projeto. Sem SDK .NET, estes cruzamentos sao
a unica rede de seguranca antes de o Arthur rodar no Visual Studio."""

import os
import re
import sys


def limpar(texto):
    """Tira strings PRIMEIRO, depois comentarios.

    Na ordem inversa o // dentro de "http://..." comeria o resto da linha
    e o contador de parenteses daria falso positivo. Esse erro ja me
    pegou uma vez na versao 2.
    """
    saida = []
    i = 0
    n = len(texto)
    while i < n:
        c = texto[i]

        if c == '@' and i + 1 < n and texto[i + 1] == '"':      # string verbatim
            i += 2
            while i < n:
                if texto[i] == '"':
                    if i + 1 < n and texto[i + 1] == '"':
                        i += 2
                        continue
                    i += 1
                    break
                i += 1
            saida.append('""')
            continue

        if c == '"':                                            # string normal
            i += 1
            while i < n:
                if texto[i] == '\\':
                    i += 2
                    continue
                if texto[i] == '"':
                    i += 1
                    break
                i += 1
            saida.append('""')
            continue

        if c == "'":                                            # caractere
            i += 1
            while i < n:
                if texto[i] == '\\':
                    i += 2
                    continue
                if texto[i] == "'":
                    i += 1
                    break
                i += 1
            saida.append("''")
            continue

        if c == '/' and i + 1 < n and texto[i + 1] == '/':      # comentario de linha
            while i < n and texto[i] != '\n':
                i += 1
            continue

        if c == '/' and i + 1 < n and texto[i + 1] == '*':      # comentario de bloco
            i += 2
            while i + 1 < n and not (texto[i] == '*' and texto[i + 1] == '/'):
                i += 1
            i += 2
            continue

        saida.append(c)
        i += 1

    return ''.join(saida)


def arquivos_cs(raiz):
    for pasta, _, nomes in os.walk(raiz):
        if any(p in pasta for p in ('bin', 'obj', '.vs', '.git')):
            continue
        for nome in sorted(nomes):
            if nome.endswith('.cs'):
                yield os.path.join(pasta, nome)


def main():
    problemas = []
    conferidos = 0

    # ---------------------------------------------------------------
    # 1) EQUILIBRIO DE CHAVES E PARENTESES
    # ---------------------------------------------------------------
    for raiz in ('desktop', 'web'):
        for caminho in arquivos_cs(raiz):
            codigo = limpar(open(caminho, encoding='utf-8-sig').read())
            conferidos += 1

            dc = codigo.count('{') - codigo.count('}')
            dp = codigo.count('(') - codigo.count(')')

            if dc:
                problemas.append(f'{caminho}: chaves desbalanceadas ({dc:+d})')
            if dp:
                problemas.append(f'{caminho}: parenteses desbalanceados ({dp:+d})')

    # ---------------------------------------------------------------
    # 2) NENHUM RESQUICIO DE "TUTOR"
    # ---------------------------------------------------------------
    # So CODIGO. Os .md e .txt documentam a renomeacao e precisam citar
    # o nome antigo - ali a mencao e correta, nao resquicio.
    for raiz in ('desktop', 'web'):
        for pasta, _, nomes in os.walk(raiz):
            if any(p in pasta for p in ('bin', 'obj', '.vs', '.git')):
                continue
            for nome in nomes:
                if not nome.endswith(('.cs', '.cshtml', '.sql')):
                    continue

                caminho = os.path.join(pasta, nome)
                texto = open(caminho, encoding='utf-8-sig', errors='replace').read()

                dentro_de_bloco = False

                for numero, contexto in enumerate(texto.splitlines(), start=1):
                    despido = contexto.strip()

                    # Acompanha comentarios de bloco /* ... */, que o
                    # cabecalho destes arquivos usa bastante.
                    if not dentro_de_bloco and '/*' in despido and '*/' not in despido:
                        dentro_de_bloco = True
                        continue
                    if dentro_de_bloco:
                        if '*/' in despido:
                            dentro_de_bloco = False
                        continue

                    if not re.search(r'(?i)tutor', contexto):
                        continue

                    # Comentario de linha explicando a renomeacao
                    if despido.startswith(('//', '--', '*', '@*')):
                        continue

                    # O UPDATE da migracao PRECISA citar 'TUTOR': e o valor
                    # antigo que ele procura no banco para converter.
                    if "Perfil = 'TUTOR'" in contexto:
                        continue

                    problemas.append(
                        f'{caminho}:{numero}: sobrou "tutor" -> {despido[:70]}')

    # ---------------------------------------------------------------
    # 3) A SENHA NAO PODE MAIS SER COMPARADA COMO TEXTO
    # ---------------------------------------------------------------
    padroes_proibidos = [
        (r'afr_Senha\s*!=\s*afr_senha', 'comparacao de senha em texto'),
        (r'afr_senhaDigitada\s*!=\s*afr_logado\.afr_Senha', 'comparacao de senha em texto'),
        # Random e previsivel (semeado pelo relogio). Para sortear
        # qualquer coisa ligada a seguranca, so RandomNumberGenerator.
        (r'new\s+Random\s*\(', 'new Random() - use RandomNumberGenerator'),
    ]
    for raiz in ('desktop', 'web'):
        for caminho in arquivos_cs(raiz):
            codigo = limpar(open(caminho, encoding='utf-8-sig').read())
            for padrao, descricao in padroes_proibidos:
                if re.search(padrao, codigo):
                    problemas.append(f'{caminho}: {descricao}')

    # ---------------------------------------------------------------
    # 5) INJECAO DE DEPENDENCIA (so a web)
    #
    # Todo tipo *Service pedido num construtor de servico ou de
    # controller precisa estar registrado no Program.cs. Se faltar, o
    # projeto COMPILA e so quebra quando a pagina e aberta - o erro
    # mais chato de achar, porque nao aparece na build.
    # ---------------------------------------------------------------
    caminho_program = 'web/PlataformaCursosWeb/Program.cs'

    if os.path.exists(caminho_program):
        program = open(caminho_program, encoding='utf-8-sig').read()
        registrados = set(re.findall(r'Add(?:Scoped|Singleton|Transient)<(\w+)>', program))

        pedidos = {}
        for caminho in list(arquivos_cs('web')):
            if caminho.endswith('Program.cs'):
                continue
            codigo = limpar(open(caminho, encoding='utf-8-sig').read())

            # public NomeClasse( ... ) { -> construtor
            for m in re.finditer(r'public\s+(\w+)\s*\(([^)]*)\)\s*(?::\s*base\([^)]*\)\s*)?\{',
                                 codigo):
                parametros = m.group(2)
                for tipo in re.findall(r'(\w+(?:Service|SessaoAtual))\s+\w+', parametros):
                    pedidos.setdefault(tipo, caminho)

        for tipo, onde in sorted(pedidos.items()):
            if tipo not in registrados:
                problemas.append(
                    f'{onde}: pede {tipo} no construtor, mas Program.cs nao registra')

    # ---------------------------------------------------------------
    # 6) CONTROLES x DESIGNER (so o desktop)
    #
    # Todo afr_lbl/afr_btn/afr_txt/afr_cmb/afr_chk/afr_lst/afr_grp usado
    # no Frm*.cs precisa estar DECLARADO no Frm*.Designer.cs, e todo
    # controle declarado precisa entrar em Controls.Add, senao ele
    # existe mas nunca aparece na tela.
    #
    # CUIDADO (falso positivo que ja me pegou na versao 2): os metodos
    # de evento tem nome parecido com os controles (afr_btnSalvar_Click
    # casa com o padrao afr_btn*). Por isso os nomes de metodo sao
    # descontados antes da comparacao.
    # ---------------------------------------------------------------
    PREFIXOS = r'afr_(?:lbl|btn|txt|cmb|chk|lst|grp|dtp|num|pnl|barra)\w*'

    for caminho in arquivos_cs('desktop'):
        if '.Designer.cs' in caminho or not os.path.basename(caminho).startswith('Frm'):
            continue

        designer = caminho.replace('.cs', '.Designer.cs')
        if not os.path.exists(designer):
            continue

        codigo = limpar(open(caminho, encoding='utf-8-sig').read())
        codigo_designer = limpar(open(designer, encoding='utf-8-sig').read())

        usados = set(re.findall(PREFIXOS, codigo))

        # desconta nomes de metodo (afr_btnX_Click, etc.)
        metodos = set(re.findall(r'(?:void|bool|int|string)\s+(' + PREFIXOS + r')\s*\(',
                                 codigo))
        metodos |= set(re.findall(r'(' + PREFIXOS + r')_\w+\s*\(', codigo))
        usados -= metodos
        usados = {u for u in usados if not re.search(r'_(?:Click|Load|Changed|Closing|Enter|Leave|KeyDown)$', u)}

        declarados = set(re.findall(
            r'private\s+System\.Windows\.Forms\.\w+\s+(' + PREFIXOS + r')\s*;',
            codigo_designer))

        adicionados = set(re.findall(
            r'Controls\.Add\(this\.(' + PREFIXOS + r')\)', codigo_designer))

        faltando = usados - declarados
        if faltando:
            problemas.append(
                f'{caminho}: controle usado mas nao declarado no Designer: {sorted(faltando)}')

        orfaos = declarados - adicionados
        if orfaos:
            problemas.append(
                f'{designer}: declarado mas nunca em Controls.Add: {sorted(orfaos)}')

    # ---------------------------------------------------------------
    # 7) VIEWS x VIEWMODELS (so a web)
    #
    # Toda propriedade Model.afr_X usada numa .cshtml precisa existir
    # na classe declarada em @model. Normalmente o compilador do Razor
    # pegaria isso, mas aqui nao ha compilador - e este e justamente o
    # erro que so apareceria com a pagina aberta na apresentacao.
    # ---------------------------------------------------------------
    pasta_views = 'web/PlataformaCursosWeb/Views'
    caminho_vm = 'web/PlataformaCursosWeb/Models/ViewModels.cs'

    if os.path.isdir(pasta_views) and os.path.exists(caminho_vm):
        fonte_vm = limpar(open(caminho_vm, encoding='utf-8-sig').read())

        # propriedades e metodos publicos de cada classe do arquivo
        membros = {}
        for m in re.finditer(r'public\s+class\s+(\w+)', fonte_vm):
            nome = m.group(1)
            inicio = m.end()
            seguinte = fonte_vm.find('public class ', inicio)
            corpo = fonte_vm[inicio:seguinte if seguinte > 0 else len(fonte_vm)]

            membros[nome] = set(
                re.findall(r'public\s+[\w<>\[\],\s\?]+?\s+(\w+)\s*(?:\{|\()', corpo))

        for pasta, _, nomes in os.walk(pasta_views):
            for nome in sorted(nomes):
                if not nome.endswith('.cshtml'):
                    continue

                caminho = os.path.join(pasta, nome)
                texto = open(caminho, encoding='utf-8-sig').read()

                m = re.search(r'@model\s+([\w\.]+)', texto)
                if not m:
                    continue

                classe = m.group(1).split('.')[-1]
                if classe not in membros:
                    continue

                usadas = set(re.findall(r'Model\.(afr_\w+)', texto))
                faltando = usadas - membros[classe]

                if faltando:
                    problemas.append(
                        f'{caminho}: usa Model.{sorted(faltando)[0]} '
                        f'que nao existe em {classe}')

    # ---------------------------------------------------------------
    # 8) LAYOUT DOS FORMULARIOS (so o desktop)
    #
    # Sem poder abrir a janela, estas coordenadas sao editadas as
    # cegas. Duas coisas dao para conferir no texto:
    #   a) controle que cai fora da area util da janela
    #   b) dois controles de primeiro nivel ocupando o mesmo espaco
    #
    # So entram os controles de PRIMEIRO NIVEL (os que vao em
    # this.Controls.Add). O que esta dentro de um GroupBox tem
    # coordenada relativa a ele e nao da para comparar na mesma regua.
    # ---------------------------------------------------------------
    for caminho in arquivos_cs('desktop'):
        if not caminho.endswith('.Designer.cs'):
            continue

        fonte = open(caminho, encoding='utf-8-sig').read()

        m = re.search(r'this\.ClientSize = new System\.Drawing\.Size\((\d+),\s*(\d+)\)',
                      fonte)
        if not m:
            continue

        largura_janela, altura_janela = int(m.group(1)), int(m.group(2))

        primeiro_nivel = set(re.findall(r'this\.Controls\.Add\(this\.(\w+)\)', fonte))

        caixas = {}
        for nome in primeiro_nivel:
            loc = re.search(r'this\.' + nome +
                            r'\.Location = new System\.Drawing\.Point\((\d+),\s*(\d+)\)',
                            fonte)
            tam = re.search(r'this\.' + nome +
                            r'\.Size = new System\.Drawing\.Size\((\d+),\s*(\d+)\)',
                            fonte)
            if loc and tam:
                caixas[nome] = (int(loc.group(1)), int(loc.group(2)),
                                int(tam.group(1)), int(tam.group(2)))

        for nome, (x, y, w, h) in sorted(caixas.items()):
            if x + w > largura_janela or y + h > altura_janela:
                problemas.append(
                    f'{caminho}: {nome} termina em ({x + w},{y + h}) e a janela '
                    f'so tem ({largura_janela},{altura_janela})')

        nomes = sorted(caixas)
        for i in range(len(nomes)):
            for j in range(i + 1, len(nomes)):
                ax, ay, aw, ah = caixas[nomes[i]]
                bx, by, bw, bh = caixas[nomes[j]]

                sobrepoe_x = ax < bx + bw and bx < ax + aw
                sobrepoe_y = ay < by + bh and by < ay + ah

                if sobrepoe_x and sobrepoe_y:
                    problemas.append(
                        f'{caminho}: {nomes[i]} e {nomes[j]} se sobrepoem')

    # ---------------------------------------------------------------
    # 9) SIMETRIA DESKTOP x WEB
    #
    # As classes de dominio e os metodos da ponte com a API tem que existir nos
    # DOIS projetos. Se um metodo entrar so num lado, o outro para de
    # compilar quando alguem chamar - ou pior, as duas interfaces
    # passam a se comportar diferente com o mesmo banco.
    # ---------------------------------------------------------------
    PARES = [
        ('desktop/PlataformaCursos/Modelo.cs',
         'web/PlataformaCursosWeb/Models/Modelo.cs',
         r'public (?:class|static class) (\w+)'),
        ('desktop/PlataformaCursos/PonteApi.cs',
         'web/PlataformaCursosWeb/Services/PonteApi.cs',
         r'public [\w<>\[\], ]+ (\w+)\s*\('),
        ('desktop/PlataformaCursos/Servicos.cs',
         'web/PlataformaCursosWeb/Services/Servicos.cs',
         r'public class (\w+)'),
    ]

    for caminho_desk, caminho_web, padrao in PARES:
        if not (os.path.exists(caminho_desk) and os.path.exists(caminho_web)):
            continue

        desk = set(re.findall(padrao, limpar(open(caminho_desk, encoding='utf-8-sig').read())))
        web = set(re.findall(padrao, limpar(open(caminho_web, encoding='utf-8-sig').read())))

        # Sessao existe so no desktop (na web virou SessaoAtual, por
        # requisicao) - e diferenca de projeto, nao falta de simetria.
        desk.discard('Sessao')

        so_desk = desk - web
        so_web = web - desk

        if so_desk:
            problemas.append(
                f'{caminho_web}: falta o que o desktop tem: {sorted(so_desk)}')
        if so_web:
            problemas.append(
                f'{caminho_desk}: falta o que a web tem: {sorted(so_web)}')

    # ---------------------------------------------------------------
    # 11) CHAMADAS A COISAS QUE NAO EXISTEM
    #
    # Depois de uma renomeacao grande, o perigo nao e o que foi
    # trocado - e o que ficou para tras chamando o nome velho. Sem
    # compilador, isso so apareceria no Visual Studio.
    #
    # Duas conferencias, as duas apoiadas na convencao de nomes do
    # projeto (todo membro comeca com afr_):
    #   a) metodo chamado em um servico do projeto tem que existir
    #   b) propriedade .afr_Xxx usada tem que estar declarada em algum
    #      lugar do modelo
    # ---------------------------------------------------------------
    for raiz_proj, arquivos_modelo in (
            ('desktop', ['desktop/PlataformaCursos/Modelo.cs']),
            ('web', ['web/PlataformaCursosWeb/Models/Modelo.cs',
                     'web/PlataformaCursosWeb/Models/ViewModels.cs'])):

        fontes = {c: limpar(open(c, encoding='utf-8-sig').read())
                  for c in arquivos_cs(raiz_proj)}
        tudo = '\n'.join(fontes.values())

        # (a) metodos definidos em qualquer lugar do projeto
        definidos = set(re.findall(
            r'(?:public|private|protected|internal)\s+(?:static\s+|virtual\s+|override\s+|async\s+)*'
            r'[\w<>\[\],\.\? ]+?\s+(\w+)\s*\(', tudo))

        # servicos do projeto, cujas chamadas da para conferir
        SERVICOS = (r'afr_bancoService|afr_gamificacaoService|afr_progressoService|'
                    r'afr_cursoService|afr_aulaService|afr_usuarioService|'
                    r'afr_autenticacaoService|afr_logService|afr_emailService')

        # Classes do modelo, para pegar tambem chamada ESTATICA
        # (Conquista.ObterTodas(), por exemplo), que nao passa por
        # campo de servico nenhum e escaparia da regra acima.
        classes_modelo = set()
        for caminho_modelo in arquivos_modelo:
            if os.path.exists(caminho_modelo):
                classes_modelo |= set(re.findall(
                    r'public\s+(?:static\s+)?class\s+(\w+)',
                    limpar(open(caminho_modelo, encoding='utf-8-sig').read())))

        alvos_chamada = list(fontes.items())
        for pasta, _, nomes in os.walk(raiz_proj):
            if any(p in pasta for p in ('bin', 'obj', '.vs')):
                continue
            for nome in nomes:
                if nome.endswith('.cshtml'):
                    caminho = os.path.join(pasta, nome)
                    alvos_chamada.append((caminho,
                                          open(caminho, encoding='utf-8-sig').read()))

        chamadores = SERVICOS
        if classes_modelo:
            chamadores = chamadores + '|' + '|'.join(sorted(classes_modelo))

        for caminho, codigo in alvos_chamada:
            for m in re.finditer(r'(?<![\w.])(?:' + chamadores + r')\s*\.\s*(\w+)\s*\(',
                                 codigo):
                if m.group(1) not in definidos:
                    linha = codigo[:m.start()].count('\n') + 1
                    problemas.append(
                        f'{caminho}:{linha}: chama {m.group(1)}() que nao existe mais')

        # (b) propriedades do modelo
        propriedades = set()
        for caminho_modelo in arquivos_modelo:
            if os.path.exists(caminho_modelo):
                propriedades |= set(re.findall(
                    r'public\s+[\w<>\[\],\.\?]+\s+(afr_\w+)\s*\{',
                    limpar(open(caminho_modelo, encoding='utf-8-sig').read())))

        if propriedades:
            # tudo que o projeto declara com afr_, para nao acusar
            # variavel local nem campo de formulario
            declarados = set(re.findall(r'\b(afr_[A-Z]\w*)\s*(?:\{|=|;|,|\))', tudo))
            conhecidos = propriedades | declarados

            alvos = list(fontes.items())
            for pasta, _, nomes in os.walk(raiz_proj):
                if any(p in pasta for p in ('bin', 'obj', '.vs')):
                    continue
                for nome in nomes:
                    if nome.endswith('.cshtml'):
                        caminho = os.path.join(pasta, nome)
                        alvos.append((caminho,
                                      open(caminho, encoding='utf-8-sig').read()))

            for caminho, codigo in alvos:
                for m in re.finditer(r'\.\s*(afr_[A-Z]\w*)\b(?!\s*\()', codigo):
                    if m.group(1) not in conhecidos:
                        linha = codigo[:m.start()].count('\n') + 1
                        problemas.append(
                            f'{caminho}:{linha}: usa .{m.group(1)}, que nao existe no modelo')

    # ---------------------------------------------------------------
    print(f'arquivos .cs conferidos: {conferidos}')
    print()

    if problemas:
        print(f'PROBLEMAS ({len(problemas)}):')
        for p in problemas:
            print('  -', p)
        return 1

    print('Nenhum problema encontrado.')
    return 0


if __name__ == '__main__':
    sys.exit(main())
