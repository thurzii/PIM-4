import React, {
  useCallback,
  useState,
} from 'react';

import {
  Alert,
  Pressable,
  ScrollView,
  StyleSheet,
  Text,
  TextInput,
  View,
} from 'react-native';

import { useFocusEffect } from '@react-navigation/native';
import api from '../services/api';

export default function ProfessorCourseScreen({ route, navigation }) {
  const initialCourse = route.params?.course || null;
  const isEditing = route.params?.mode === 'edit' && !!initialCourse;

  const [titulo, setTitulo] = useState(initialCourse?.titulo || '');
  const [descricao, setDescricao] = useState(initialCourse?.descricao || '');
  const [cargaHoraria, setCargaHoraria] = useState(
    initialCourse ? String(initialCourse.cargaHoraria) : '0'
  );
  const [aulas, setAulas] = useState([]);
  const [aulaTitulo, setAulaTitulo] = useState('');
  const [aulaDuracao, setAulaDuracao] = useState('45');
  const [saving, setSaving] = useState(false);

  async function loadLessons() {
    if (!isEditing) return;

    try {
      const response = await api.get(`/professor/cursos/${initialCourse.id}/aulas`);
      setAulas(response.data);
    } catch (error) {
      Alert.alert('Erro', error.response?.data?.message || 'Falha ao carregar aulas.');
    }
  }

  useFocusEffect(
    useCallback(() => {
      loadLessons();
    }, [initialCourse?.id])
  );

  async function saveCourse() {
    const title = titulo.trim();
    const hours = Number(cargaHoraria);

    if (!title) {
      Alert.alert('Atenção', 'Informe o título do curso.');
      return;
    }

    if (!Number.isInteger(hours) || hours < 0) {
      Alert.alert('Atenção', 'Informe uma carga horária válida.');
      return;
    }

    setSaving(true);

    try {
      const payload = {
        titulo: title,
        descricao: descricao.trim(),
        cargaHoraria: hours,
      };

      if (isEditing) {
        await api.put(`/professor/cursos/${initialCourse.id}`, payload);
        Alert.alert('Sucesso', 'Curso atualizado.');
      } else {
        const response = await api.post('/professor/cursos', payload);
        Alert.alert('Sucesso', 'Curso criado. Agora você pode adicionar as aulas.');
        navigation.replace('ProfessorCurso', {
          mode: 'edit',
          course: {
            id: response.data.id,
            ...payload,
          },
        });
      }
    } catch (error) {
      Alert.alert('Erro', error.response?.data?.message || 'Falha ao salvar curso.');
    } finally {
      setSaving(false);
    }
  }

  async function addLesson() {
    const title = aulaTitulo.trim();
    const duration = Number(aulaDuracao);

    if (!title || title.length > 300) {
      Alert.alert('Atenção', 'Informe um título de aula com até 300 caracteres.');
      return;
    }

    if (!Number.isInteger(duration) || duration < 0) {
      Alert.alert('Atenção', 'Informe uma duração válida em minutos.');
      return;
    }

    try {
      await api.post(`/professor/cursos/${initialCourse.id}/aulas`, {
        titulo: title,
        duracaoMin: duration,
      });
      setAulaTitulo('');
      setAulaDuracao('45');
      await loadLessons();
    } catch (error) {
      Alert.alert('Erro', error.response?.data?.message || 'Falha ao adicionar aula.');
    }
  }

  function deleteLesson(lesson) {
    Alert.alert(
      'Excluir aula',
      `Deseja excluir “${lesson.titulo}”? O progresso relacionado também será removido.`,
      [
        { text: 'Cancelar', style: 'cancel' },
        {
          text: 'Excluir',
          style: 'destructive',
          onPress: async () => {
            try {
              await api.delete(`/professor/aulas/${lesson.id}`);
              await loadLessons();
            } catch (error) {
              Alert.alert('Erro', error.response?.data?.message || 'Falha ao excluir aula.');
            }
          },
        },
      ]
    );
  }

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Text style={styles.title}>{isEditing ? 'Gerenciar curso' : 'Novo curso'}</Text>

      <Text style={styles.label}>Título</Text>
      <TextInput
        style={styles.input}
        placeholder="Nome do curso"
        value={titulo}
        onChangeText={setTitulo}
        maxLength={200}
      />

      <Text style={styles.label}>Descrição</Text>
      <TextInput
        style={[styles.input, styles.textArea]}
        placeholder="Descreva o conteúdo do curso"
        value={descricao}
        onChangeText={setDescricao}
        multiline
      />

      <Text style={styles.label}>Carga horária (horas)</Text>
      <TextInput
        style={styles.input}
        placeholder="Carga horária"
        value={cargaHoraria}
        onChangeText={setCargaHoraria}
        keyboardType="numeric"
      />

      <Pressable style={[styles.primaryButton, saving && styles.disabled]} onPress={saveCourse} disabled={saving}>
        <Text style={styles.primaryButtonText}>{saving ? 'Salvando...' : 'Salvar curso'}</Text>
      </Pressable>

      {isEditing && (
        <>
          <Pressable
            style={styles.secondaryButton}
            onPress={() => navigation.navigate('ProfessorAlunos', {
              courseId: initialCourse.id,
              courseTitle: titulo,
            })}
          >
            <Text style={styles.secondaryButtonText}>Ver alunos e progresso</Text>
          </Pressable>

          <Text style={styles.sectionTitle}>Aulas do curso</Text>

          {aulas.map((lesson) => (
            <View style={styles.lesson} key={lesson.id}>
              <View style={styles.lessonInfo}>
                <Text style={styles.lessonTitle}>{lesson.ordem}. {lesson.titulo}</Text>
                <Text style={styles.lessonDuration}>{lesson.duracaoMin} min</Text>
              </View>
              <Pressable onPress={() => deleteLesson(lesson)}>
                <Text style={styles.deleteText}>Excluir</Text>
              </Pressable>
            </View>
          ))}

          <Text style={styles.sectionTitle}>Adicionar aula</Text>
          <TextInput
            style={styles.input}
            placeholder="Título da aula"
            value={aulaTitulo}
            onChangeText={setAulaTitulo}
            maxLength={300}
          />
          <TextInput
            style={styles.input}
            placeholder="Duração em minutos"
            value={aulaDuracao}
            onChangeText={setAulaDuracao}
            keyboardType="numeric"
          />
          <Pressable style={styles.primaryButton} onPress={addLesson}>
            <Text style={styles.primaryButtonText}>Adicionar aula</Text>
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
  },
  content: {
    padding: 18,
    paddingBottom: 35,
  },
  title: {
    fontSize: 25,
    fontWeight: '800',
    marginBottom: 20,
  },
  label: {
    fontWeight: '700',
    color: '#344054',
    marginBottom: 6,
  },
  input: {
    backgroundColor: '#fff',
    borderWidth: 1,
    borderColor: '#d0d5dd',
    borderRadius: 10,
    padding: 13,
    marginBottom: 14,
  },
  textArea: {
    minHeight: 100,
    textAlignVertical: 'top',
  },
  primaryButton: {
    backgroundColor: '#2563eb',
    borderRadius: 10,
    padding: 15,
    alignItems: 'center',
    marginTop: 3,
  },
  primaryButtonText: {
    color: '#fff',
    fontWeight: '800',
  },
  disabled: {
    opacity: 0.6,
  },
  secondaryButton: {
    backgroundColor: '#fff',
    borderWidth: 1,
    borderColor: '#2563eb',
    borderRadius: 10,
    padding: 14,
    alignItems: 'center',
    marginTop: 14,
  },
  secondaryButtonText: {
    color: '#2563eb',
    fontWeight: '800',
  },
  sectionTitle: {
    fontSize: 19,
    fontWeight: '800',
    marginTop: 25,
    marginBottom: 12,
  },
  lesson: {
    backgroundColor: '#fff',
    borderRadius: 10,
    padding: 14,
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 8,
  },
  lessonInfo: {
    flex: 1,
  },
  lessonTitle: {
    fontWeight: '700',
  },
  lessonDuration: {
    color: '#667085',
    marginTop: 4,
  },
  deleteText: {
    color: '#dc2626',
    fontWeight: '700',
    padding: 5,
  },
});
