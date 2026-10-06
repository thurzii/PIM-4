import React, {
  useCallback,
  useState,
} from 'react';

import {
  Alert,
  FlatList,
  Pressable,
  RefreshControl,
  StyleSheet,
  Text,
  TextInput,
  View,
} from 'react-native';

import { useFocusEffect } from '@react-navigation/native';
import api from '../services/api';

export default function AdminScreen({ navigation }) {
  const [data, setData] = useState([]);
  const [busca, setBusca] = useState('');
  const [loading, setLoading] = useState(false);

  async function load(term = '') {
    setLoading(true);

    try {
      const response = await api.get('/admin/cursos', {
        params: {
          busca: term,
        },
      });

      setData(response.data);
    } catch (error) {
      Alert.alert(
        'Erro',
        error.response?.data?.message ||
          'Falha ao carregar.'
      );
    } finally {
      setLoading(false);
    }
  }

  useFocusEffect(
    useCallback(() => {
      load(busca);
    }, [])
  );

  function novo() {
    navigation.navigate('AdminCurso', {
      mode: 'new',
    });
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <View>
          <Text style={styles.title}>
            Administração
          </Text>

          <Text style={styles.subtitle}>
            Gerenciamento de cursos
          </Text>
        </View>

        <Pressable
          style={styles.addButton}
          onPress={novo}
        >
          <Text style={styles.addButtonText}>
            + Novo
          </Text>
        </Pressable>
      </View>

      <TextInput
        style={styles.search}
        placeholder="Buscar..."
        value={busca}
        onChangeText={setBusca}
        onSubmitEditing={() => load(busca)}
      />

      <FlatList
        data={data}
        keyExtractor={(item) => String(item.id)}
        refreshControl={
          <RefreshControl
            refreshing={loading}
            onRefresh={() => load(busca)}
          />
        }
        renderItem={({ item }) => (
          <Pressable
            style={styles.card}
            onPress={() =>
              navigation.navigate('AdminCurso', {
                mode: 'edit',
                course: item,
              })
            }
          >
            <Text style={styles.courseTitle}>
              {item.titulo}
            </Text>

            <Text>
              {item.cargaHoraria}h • {item.totalAulas} aulas
            </Text>

            <Text
              style={[
                styles.status,
                {
                  color: item.ativo
                    ? '#16a34a'
                    : '#777',
                },
              ]}
            >
              {item.ativo ? 'Ativo' : 'Inativo'}
            </Text>
          </Pressable>
        )}
      />
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
    justifyContent: 'space-between',
    alignItems: 'center',
  },

  title: {
    fontSize: 25,
    fontWeight: '800',
  },

  subtitle: {
    color: '#667085',
  },

  addButton: {
    backgroundColor: '#2563eb',
    padding: 11,
    borderRadius: 9,
  },

  addButtonText: {
    color: '#fff',
    fontWeight: '700',
  },

  search: {
    backgroundColor: '#fff',
    padding: 13,
    borderRadius: 10,
    borderWidth: 1,
    borderColor: '#ddd',
    marginVertical: 15,
  },

  card: {
    backgroundColor: '#fff',
    padding: 16,
    borderRadius: 12,
    marginBottom: 10,
  },

  courseTitle: {
    fontWeight: '800',
    fontSize: 16,
    marginBottom: 5,
  },

  status: {
    marginTop: 5,
  },
});