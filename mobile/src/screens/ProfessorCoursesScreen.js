import React, {
  useCallback,
  useState,
} from 'react';

import {
  ActivityIndicator,
  Alert,
  FlatList,
  Pressable,
  RefreshControl,
  StyleSheet,
  Text,
  View,
} from 'react-native';

import { useFocusEffect } from '@react-navigation/native';
import api from '../services/api';

export default function ProfessorCoursesScreen({ navigation }) {
  const [courses, setCourses] = useState([]);
  const [loading, setLoading] = useState(true);

  async function load() {
    setLoading(true);

    try {
      const response = await api.get('/professor/cursos');
      setCourses(response.data);
    } catch (error) {
      Alert.alert(
        'Erro',
        error.response?.data?.message || 'Falha ao carregar seus cursos.'
      );
    } finally {
      setLoading(false);
    }
  }

  useFocusEffect(
    useCallback(() => {
      load();
    }, [])
  );

  function novoCurso() {
    navigation.navigate('ProfessorCurso', { mode: 'new' });
  }

  function abrirCurso(course) {
    navigation.navigate('ProfessorCurso', {
      mode: 'edit',
      course,
    });
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <View style={styles.heading}>
          <Text style={styles.title}>Meus cursos</Text>
          <Text style={styles.subtitle}>Gerencie seus cursos e acompanhe seus alunos.</Text>
        </View>

        <Pressable style={styles.addButton} onPress={novoCurso}>
          <Text style={styles.addButtonText}>+ Novo</Text>
        </Pressable>
      </View>

      {loading ? (
        <ActivityIndicator style={styles.loading} size="large" />
      ) : (
        <FlatList
          data={courses}
          keyExtractor={(item) => String(item.id)}
          refreshControl={
            <RefreshControl refreshing={loading} onRefresh={load} />
          }
          renderItem={({ item }) => (
            <Pressable style={styles.card} onPress={() => abrirCurso(item)}>
              <Text style={styles.courseTitle}>{item.titulo}</Text>
              <Text style={styles.description} numberOfLines={2}>{item.descricao}</Text>
              <View style={styles.metrics}>
                <Text style={styles.metric}>{item.cargaHoraria}h</Text>
                <Text style={styles.metric}>{item.totalAulas} aulas</Text>
                <Text style={styles.metric}>{item.totalAlunos} alunos</Text>
              </View>
            </Pressable>
          )}
          ListEmptyComponent={
            <Text style={styles.empty}>Você ainda não possui cursos. Toque em “+ Novo” para criar o primeiro.</Text>
          }
        />
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 18,
    backgroundColor: '#f4f6f8',
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginBottom: 18,
  },
  heading: {
    flex: 1,
    paddingRight: 10,
  },
  title: {
    fontSize: 25,
    fontWeight: '800',
  },
  subtitle: {
    color: '#667085',
    marginTop: 3,
  },
  addButton: {
    backgroundColor: '#2563eb',
    paddingVertical: 11,
    paddingHorizontal: 13,
    borderRadius: 9,
  },
  addButtonText: {
    color: '#fff',
    fontWeight: '700',
  },
  loading: {
    marginTop: 35,
  },
  card: {
    backgroundColor: '#fff',
    padding: 16,
    borderRadius: 12,
    marginBottom: 11,
  },
  courseTitle: {
    fontSize: 16,
    fontWeight: '800',
  },
  description: {
    color: '#667085',
    marginTop: 5,
  },
  metrics: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    marginTop: 12,
  },
  metric: {
    color: '#2563eb',
    fontWeight: '700',
    marginRight: 14,
  },
  empty: {
    textAlign: 'center',
    color: '#667085',
    marginTop: 45,
    lineHeight: 22,
  },
});
