import React, {
  useCallback,
  useState,
} from 'react';

import {
  ActivityIndicator,
  FlatList,
  RefreshControl,
  StyleSheet,
  Text,
  TextInput,
  View,
} from 'react-native';

import { useFocusEffect } from '@react-navigation/native';

import api from '../services/api';
import CourseCard from '../components/CourseCard';

export default function CoursesScreen({ navigation }) {
  const [courses, setCourses] = useState([]);
  const [busca, setBusca] = useState('');
  const [loading, setLoading] = useState(true);

  async function load(term = '') {
    setLoading(true);

    try {
      const response = await api.get('/cursos', {
        params: {
          busca: term,
        },
      });

      setCourses(response.data);
    } finally {
      setLoading(false);
    }
  }

  useFocusEffect(
    useCallback(() => {
      load(busca);
    }, [])
  );

  return (
    <View style={styles.container}>
      <Text style={styles.title}>
        Olá! 👋
      </Text>

      <Text style={styles.subtitle}>
        Encontre um curso para continuar seus estudos.
      </Text>

      <TextInput
        style={styles.search}
        placeholder="Buscar curso..."
        value={busca}
        onChangeText={setBusca}
        onSubmitEditing={() => load(busca)}
      />

      {loading ? (
        <ActivityIndicator
          style={styles.loading}
        />
      ) : (
        <FlatList
          data={courses}
          keyExtractor={(item) => String(item.id)}
          renderItem={({ item }) => (
            <CourseCard
              course={item}
              onPress={() =>
                navigation.navigate(
                  'CursoDetalhes',
                  {
                    id: item.id,
                  }
                )
              }
            />
          )}
          refreshControl={
            <RefreshControl
              refreshing={loading}
              onRefresh={() => load(busca)}
            />
          }
          ListEmptyComponent={
            <Text style={styles.empty}>
              Nenhum curso encontrado.
            </Text>
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
    fontSize: 28,
    fontWeight: '800',
  },

  subtitle: {
    color: '#667085',
    marginBottom: 16,
  },

  search: {
    backgroundColor: '#fff',
    padding: 13,
    borderRadius: 10,
    borderWidth: 1,
    borderColor: '#ddd',
    marginBottom: 15,
  },

  loading: {
    marginTop: 30,
  },

  empty: {
    textAlign: 'center',
    marginTop: 40,
    color: '#777',
  },
});