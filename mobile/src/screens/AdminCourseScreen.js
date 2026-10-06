import React, { useEffect, useState } from 'react';
import {
  Alert,
  Pressable,
  ScrollView,
  StyleSheet,
  Switch,
  Text,
  TextInput,
  View,
} from 'react-native';

import api from '../services/api';

export default function AdminCourseScreen({ route, navigation }) {
  const edit = route.params?.mode === 'edit';
  const initial = route.params?.course;

  const [titulo, setTitulo] = useState(initial?.titulo || '');
  const [descricao, setDescricao] = useState(initial?.descricao || '');
  const [carga, setCarga] = useState(
    String(initial?.cargaHoraria || '')
  );
  const [ativo, setAtivo] = useState(initial?.ativo !== false);

  const [aulas, setAulas] = useState([]);
  const [aulaTitulo, setAulaTitulo] = useState('');
  const [aulaDuracao, setAulaDuracao] = useState('45');

  useEffect(() => {
    if (edit) {
      load();
    }
  }, []);

  async function load() {
    try {
      const response = await api.get(`/cursos/${initial.id}`);
      setAulas(response.data.aulas);
    } catch (error) {
      Alert.alert(
        'Erro',
        'Não foi possível carregar as aulas.'
      );
    }
  }

  async function salvar() {
    try {
      const dadosCurso = {
        titulo,
        descricao,
        cargaHoraria: Number(carga),
        ativo,
      };

      if (edit) {
        await api.put(
          `/admin/cursos/${initial.id}`,
          dadosCurso
        );
      } else {
        await api.post(
          '/admin/cursos',
          dadosCurso
        );
      }

      Alert.alert(
        'Sucesso',
        edit ? 'Curso atualizado.' : 'Curso criado.',
        [
          {
            text: 'OK',
            onPress: () => navigation.goBack(),
          },
        ]
      );
    } catch (error) {
      Alert.alert(
        'Erro',
        error.response?.data?.message ||
          'Falha ao salvar.'
      );
    }
  }

  async function addAula() {
    if (!edit) {
      Alert.alert(
        'Atenção',
        'Salve o curso antes de adicionar aulas.'
      );
      return;
    }

    try {
      await api.post(
        `/admin/cursos/${initial.id}/aulas`,
        {
          titulo: aulaTitulo,
          duracaoMin: Number(aulaDuracao),
        }
      );

      setAulaTitulo('');
      await load();
    } catch (error) {
      Alert.alert(
        'Erro',
        error.response?.data?.message ||
          'Falha ao criar aula.'
      );
    }
  }

  async function delAula(id) {
    try {
      await api.delete(`/admin/aulas/${id}`);
      await load();
    } catch (error) {
      Alert.alert(
        'Erro',
        error.response?.data?.message ||
          'Falha ao excluir aula.'
      );
    }
  }

  function excluir() {
    Alert.alert(
      'Excluir',
      'Deseja excluir este curso?',
      [
        {
          text: 'Cancelar',
        },
        {
          text: 'Excluir',
          style: 'destructive',
          onPress: async () => {
            try {
              await api.delete(
                `/admin/cursos/${initial.id}`
              );

              navigation.goBack();
            } catch (error) {
              Alert.alert(
                'Erro',
                error.response?.data?.message ||
                  'Falha ao excluir.'
              );
            }
          },
        },
      ]
    );
  }

  return (
    <ScrollView style={styles.container}>
      <Text style={styles.title}>
        {edit ? 'Editar curso' : 'Novo curso'}
      </Text>

      <TextInput
        style={styles.input}
        placeholder="Título"
        value={titulo}
        onChangeText={setTitulo}
      />

      <TextInput
        style={[styles.input, styles.textArea]}
        placeholder="Descrição"
        multiline
        value={descricao}
        onChangeText={setDescricao}
      />

      <TextInput
        style={styles.input}
        placeholder="Carga horária"
        keyboardType="numeric"
        value={carga}
        onChangeText={setCarga}
      />

      <View style={styles.switchContainer}>
        <Text>Curso ativo</Text>

        <Switch
          value={ativo}
          onValueChange={setAtivo}
        />
      </View>

      <Pressable
        style={styles.button}
        onPress={salvar}
      >
        <Text style={styles.buttonText}>
          Salvar curso
        </Text>
      </Pressable>

      {edit && (
        <>
          <Text style={styles.heading}>
            Aulas
          </Text>

          {aulas.map((aula) => (
            <View
              style={styles.lesson}
              key={aula.id}
            >
              <View style={styles.lessonInfo}>
                <Text style={styles.lessonTitle}>
                  {aula.ordem}. {aula.titulo}
                </Text>

                <Text>
                  {aula.duracaoMin} min
                </Text>
              </View>

              <Pressable
                onPress={() => delAula(aula.id)}
              >
                <Text style={styles.deleteText}>
                  Excluir
                </Text>
              </Pressable>
            </View>
          ))}

          <TextInput
            style={styles.input}
            placeholder="Título da nova aula"
            value={aulaTitulo}
            onChangeText={setAulaTitulo}
          />

          <TextInput
            style={styles.input}
            placeholder="Duração em minutos"
            keyboardType="numeric"
            value={aulaDuracao}
            onChangeText={setAulaDuracao}
          />

          <Pressable
            style={styles.secondaryButton}
            onPress={addAula}
          >
            <Text style={styles.secondaryButtonText}>
              Adicionar aula
            </Text>
          </Pressable>

          <Pressable
            style={styles.dangerButton}
            onPress={excluir}
          >
            <Text style={styles.buttonText}>
              Excluir curso
            </Text>
          </Pressable>
        </>
      )}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f4f6f8',
    padding: 18,
  },

  title: {
    fontSize: 25,
    fontWeight: '800',
    marginBottom: 18,
  },

  input: {
    backgroundColor: '#fff',
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 10,
    padding: 13,
    marginBottom: 10,
  },

  textArea: {
    minHeight: 100,
    textAlignVertical: 'top',
  },

  switchContainer: {
    backgroundColor: '#fff',
    padding: 14,
    borderRadius: 10,
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 12,
  },

  button: {
    backgroundColor: '#2563eb',
    padding: 15,
    borderRadius: 10,
    alignItems: 'center',
  },

  buttonText: {
    color: '#fff',
    fontWeight: '700',
  },

  heading: {
    fontSize: 20,
    fontWeight: '800',
    marginVertical: 18,
  },

  lesson: {
    backgroundColor: '#fff',
    padding: 14,
    borderRadius: 10,
    flexDirection: 'row',
    marginBottom: 8,
  },

  lessonInfo: {
    flex: 1,
  },

  lessonTitle: {
    fontWeight: '700',
  },

  deleteText: {
    color: '#dc2626',
    fontWeight: '700',
  },

  secondaryButton: {
    backgroundColor: '#fff',
    padding: 14,
    borderRadius: 10,
    borderWidth: 1,
    borderColor: '#2563eb',
    alignItems: 'center',
  },

  secondaryButtonText: {
    color: '#2563eb',
    fontWeight: '700',
  },

  dangerButton: {
    backgroundColor: '#dc2626',
    padding: 14,
    borderRadius: 10,
    alignItems: 'center',
    marginVertical: 20,
  },
});