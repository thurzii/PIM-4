import React, {
  useCallback,
  useState,
} from 'react';

import {
  ActivityIndicator,
  Alert,
  FlatList,
  RefreshControl,
  StyleSheet,
  Text,
  View,
} from 'react-native';

import { useFocusEffect } from '@react-navigation/native';
import api from '../services/api';
import ProgressBar from '../components/ProgressBar';

export default function ProfessorStudentsScreen({ route }) {
  const { courseId, courseTitle } = route.params;
  const [students, setStudents] = useState([]);
  const [loading, setLoading] = useState(true);

  async function load() {
    setLoading(true);

    try {
      const response = await api.get(`/professor/cursos/${courseId}/alunos`);
      setStudents(response.data);
    } catch (error) {
      setStudents([]);
      Alert.alert(
        'Erro',
        error.response?.data?.message || 'Falha ao carregar os alunos.'
      );
    } finally {
      setLoading(false);
    }
  }

  useFocusEffect(
    useCallback(() => {
      load();
    }, [courseId])
  );

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Alunos</Text>
      <Text style={styles.subtitle}>{courseTitle}</Text>

      {loading ? (
        <ActivityIndicator style={styles.loading} size="large" />
      ) : (
        <FlatList
          data={students}
          keyExtractor={(item) => String(item.id)}
          refreshControl={<RefreshControl refreshing={loading} onRefresh={load} />}
          renderItem={({ item }) => (
            <View style={styles.card}>
              <Text style={styles.name}>{item.nome}</Text>
              <Text style={styles.email}>{item.email}</Text>
              <Text style={styles.enrollment}>
                Matrícula: {new Date(item.dataMatricula).toLocaleDateString('pt-BR')}
              </Text>
              <View style={styles.progressHeader}>
                <Text style={styles.progressLabel}>Progresso</Text>
                <Text style={styles.progressValue}>{item.progresso}%</Text>
              </View>
              <ProgressBar value={item.progresso} />
              <Text style={styles.lessonCount}>
                {item.aulasConcluidas} de {item.totalAulas} aulas concluídas
              </Text>
            </View>
          )}
          ListEmptyComponent={
            <Text style={styles.empty}>Ainda não há alunos matriculados neste curso.</Text>
          }
        />
      )}
    </View>
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
  },
  subtitle: {
    color: '#667085',
    marginBottom: 18,
    marginTop: 3,
  },
  loading: {
    marginTop: 35,
  },
  card: {
    backgroundColor: '#fff',
    padding: 16,
    borderRadius: 12,
    marginBottom: 12,
  },
  name: {
    fontSize: 16,
    fontWeight: '800',
  },
  email: {
    color: '#667085',
    marginTop: 3,
  },
  enrollment: {
    color: '#667085',
    fontSize: 12,
    marginTop: 8,
  },
  progressHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginTop: 15,
    marginBottom: 7,
  },
  progressLabel: {
    fontWeight: '700',
  },
  progressValue: {
    color: '#2563eb',
    fontWeight: '800',
  },
  lessonCount: {
    color: '#667085',
    fontSize: 12,
    marginTop: 7,
  },
  empty: {
    color: '#667085',
    textAlign: 'center',
    marginTop: 45,
  },
});
