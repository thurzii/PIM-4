import React, {
  useCallback,
  useState,
} from 'react';

import {
  Alert,
  FlatList,
  RefreshControl,
  StyleSheet,
  Switch,
  Text,
  TextInput,
  View,
} from 'react-native';

import { useFocusEffect } from '@react-navigation/native';
import api from '../services/api';

export default function AdminUsersScreen() {
  const [data, setData] = useState([]);
  const [busca, setBusca] = useState('');
  const [loading, setLoading] = useState(false);

  async function load(term = '') {
    setLoading(true);

    try {
      const response = await api.get('/admin/usuarios', {
        params: {
          busca: term,
        },
      });

      setData(response.data);
    } catch (error) {
      Alert.alert(
        'Erro',
        error.response?.data?.message ||
          'Falha ao carregar usuários.'
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

  async function toggle(item, value) {
    try {
      await api.put(
        `/admin/usuarios/${item.id}/status`,
        {
          ativo: value,
        }
      );

      setData((currentData) =>
        currentData.map((user) =>
          user.id === item.id
            ? {
                ...user,
                ativo: value,
              }
            : user
        )
      );
    } catch (error) {
      Alert.alert(
        'Erro',
        error.response?.data?.message ||
          'Falha ao atualizar.'
      );
    }
  }

  return (
    <View style={styles.container}>
      <Text style={styles.title}>
        Usuários
      </Text>

      <TextInput
        style={styles.search}
        placeholder="Buscar nome ou email"
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
          <View style={styles.card}>
            <View style={styles.userInfo}>
              <Text style={styles.name}>
                {item.nome}
              </Text>

              <Text style={styles.email}>
                {item.email}
              </Text>

              <Text style={styles.role}>
                {item.perfil}
              </Text>
            </View>

            <Switch
              value={!!item.ativo}
              onValueChange={(value) =>
                toggle(item, value)
              }
            />
          </View>
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

  title: {
    fontSize: 25,
    fontWeight: '800',
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
    padding: 15,
    borderRadius: 12,
    marginBottom: 10,
    flexDirection: 'row',
    alignItems: 'center',
  },

  userInfo: {
    flex: 1,
  },

  name: {
    fontWeight: '800',
  },

  email: {
    color: '#667085',
    marginTop: 3,
  },

  role: {
    fontSize: 11,
    color: '#2563eb',
    marginTop: 4,
  },
});