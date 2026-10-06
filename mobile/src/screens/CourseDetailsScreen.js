import React, {
  useCallback,
  useState,
} from 'react';

import {
  Alert,
  RefreshControl,
  ScrollView,
  StyleSheet,
  Switch,
  Text,
  View,
} from 'react-native';

import { useFocusEffect } from '@react-navigation/native';

import api from '../services/api';
import ProgressBar from '../components/ProgressBar';

export default function CourseDetailsScreen({ route }) {
  const { id } = route.params;

  const [data, setData] = useState(null);
  const [refreshing, setRefreshing] = useState(false);

  async function load() {
    setRefreshing(true);

    try {
      const response = await api.get(`/cursos/${id}`);
      setData(response.data);
    } catch (error) {
      Alert.alert(
        'Erro',
        error.response?.data?.message ||
          'Falha ao carregar curso.'
      );
    } finally {
      setRefreshing(false);
    }
  }

  useFocusEffect(
    useCallback(() => {
      load();
    }, [id])
  );

  async function toggle(aula, value) {
    try {
      await api.put(
        `/cursos/${id}/aulas/${aula.id}/progresso`,
        {
          concluida: value,
        }
      );

      setData((currentData) => {
        const aulasAtualizadas = currentData.aulas.map(
          (item) =>
            item.id === aula.id
              ? {
                  ...item,
                  concluida: value,
                }
              : item
        );

        const concluidas = aulasAtualizadas.filter(
          (item) => item.concluida
        ).length;

        const progresso = aulasAtualizadas.length
          ? Math.round(
              (concluidas * 100) /
                aulasAtualizadas.length
            )
          : 0;

        return {
          ...currentData,
          aulas: aulasAtualizadas,
          progresso,
          concluidas,
        };
      });
    } catch (error) {
      Alert.alert(
        'Erro',
        error.response?.data?.message ||
          'Falha ao sincronizar progresso.'
      );
    }
  }

  if (!data) {
    return (
      <View style={styles.center}>
        <Text>Carregando...</Text>
      </View>
    );
  }

  return (
    <ScrollView
      style={styles.container}
      refreshControl={
        <RefreshControl
          refreshing={refreshing}
          onRefresh={load}
        />
      }
    >
      <Text style={styles.title}>
        {data.titulo}
      </Text>

      <Text style={styles.description}>
        {data.descricao}
      </Text>

      <Text style={styles.meta}>
        {data.cargaHoraria} horas • {data.totalAulas} aulas
      </Text>

      <View style={styles.progressContainer}>
        <ProgressBar value={data.progresso} />

        <Text style={styles.count}>
          {data.concluidas} de {data.totalAulas} aulas concluídas
        </Text>
      </View>

      <Text style={styles.heading}>
        Aulas
      </Text>

      {data.aulas.map((aula) => (
        <View
          style={styles.lesson}
          key={aula.id}
        >
          <View style={styles.lessonInfo}>
            <Text style={styles.lessonTitle}>
              {aula.ordem}. {aula.titulo}
            </Text>

            <Text style={styles.duration}>
              {aula.duracaoMin} min
            </Text>
          </View>

          <Switch
            value={!!aula.concluida}
            onValueChange={(value) =>
              toggle(aula, value)
            }
          />
        </View>
      ))}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f4f6f8',
    padding: 18,
  },

  center: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },

  title: {
    fontSize: 27,
    fontWeight: '800',
  },

  description: {
    fontSize: 15,
    lineHeight: 22,
    color: '#555',
    marginTop: 10,
  },

  meta: {
    color: '#667085',
    marginTop: 10,
  },

  progressContainer: {
    backgroundColor: '#fff',
    padding: 16,
    borderRadius: 14,
    marginTop: 18,
  },

  count: {
    fontSize: 12,
    color: '#666',
    marginTop: 7,
  },

  heading: {
    fontSize: 20,
    fontWeight: '800',
    marginVertical: 16,
  },

  lesson: {
    backgroundColor: '#fff',
    padding: 15,
    borderRadius: 12,
    marginBottom: 10,
    flexDirection: 'row',
    alignItems: 'center',
  },

  lessonInfo: {
    flex: 1,
  },

  lessonTitle: {
    fontSize: 15,
    fontWeight: '700',
  },

  duration: {
    fontSize: 12,
    color: '#777',
    marginTop: 4,
  },
});