import React, {
  useCallback,
  useState,
} from 'react';

import {
  Alert,
  FlatList,
<<<<<<< HEAD
=======
  Pressable,
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
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
<<<<<<< HEAD
=======
  const [showForm, setShowForm] = useState(false);
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [perfil, setPerfil] = useState('ALUNO');
  const [saving, setSaving] = useState(false);
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)

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

<<<<<<< HEAD
=======
  async function createUser() {
    if (!nome.trim() || !email.trim() || !senha) {
      Alert.alert('Atenção', 'Preencha nome, email e senha.');
      return;
    }

    setSaving(true);

    try {
      await api.post('/admin/usuarios', {
        nome: nome.trim(),
        email: email.trim(),
        senha,
        perfil,
      });

      setNome('');
      setEmail('');
      setSenha('');
      setPerfil('ALUNO');
      setShowForm(false);
      await load(busca);
      Alert.alert('Sucesso', 'Usuário criado.');
    } catch (error) {
      Alert.alert('Erro', error.response?.data?.message || 'Falha ao criar usuário.');
    } finally {
      setSaving(false);
    }
  }

>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
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
<<<<<<< HEAD
      <Text style={styles.title}>
        Usuários
      </Text>
=======
      <View style={styles.header}>
        <Text style={styles.title}>Usuários</Text>
        <Pressable style={styles.addButton} onPress={() => setShowForm((value) => !value)}>
          <Text style={styles.addButtonText}>{showForm ? 'Fechar' : '+ Novo'}</Text>
        </Pressable>
      </View>

      {showForm && (
        <View style={styles.form}>
          <TextInput style={styles.formInput} placeholder="Nome completo" value={nome} onChangeText={setNome} maxLength={200} />
          <TextInput style={styles.formInput} placeholder="Email" value={email} onChangeText={setEmail} autoCapitalize="none" keyboardType="email-address" maxLength={200} />
          <TextInput style={styles.formInput} placeholder="Senha (mínimo 6 caracteres)" value={senha} onChangeText={setSenha} secureTextEntry maxLength={200} />
          <Text style={styles.roleLabel}>Perfil do usuário</Text>
          <View style={styles.roleOptions}>
            {['ALUNO', 'PROFESSOR', 'ADMIN'].map((role) => (
              <Pressable key={role} style={[styles.roleOption, perfil === role && styles.roleOptionSelected]} onPress={() => setPerfil(role)}>
                <Text style={[styles.roleOptionText, perfil === role && styles.roleOptionTextSelected]}>{role}</Text>
              </Pressable>
            ))}
          </View>
          <Pressable style={[styles.saveButton, saving && styles.disabled]} onPress={createUser} disabled={saving}>
            <Text style={styles.saveButtonText}>{saving ? 'Criando...' : 'Criar usuário'}</Text>
          </Pressable>
        </View>
      )}
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)

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

<<<<<<< HEAD
=======
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },

>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  title: {
    fontSize: 25,
    fontWeight: '800',
  },

<<<<<<< HEAD
=======
  addButton: {
    backgroundColor: '#2563eb',
    paddingVertical: 10,
    paddingHorizontal: 13,
    borderRadius: 9,
  },

  addButtonText: {
    color: '#fff',
    fontWeight: '700',
  },

  form: {
    backgroundColor: '#fff',
    padding: 14,
    borderRadius: 12,
    marginTop: 14,
  },

  formInput: {
    backgroundColor: '#f9fafb',
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 9,
    padding: 12,
    marginBottom: 9,
  },

  roleLabel: {
    fontWeight: '700',
    marginTop: 4,
    marginBottom: 8,
  },

  roleOptions: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    marginBottom: 12,
  },

  roleOption: {
    borderWidth: 1,
    borderColor: '#d0d5dd',
    borderRadius: 8,
    paddingVertical: 8,
    paddingHorizontal: 10,
    marginRight: 7,
  },

  roleOptionSelected: {
    backgroundColor: '#dbeafe',
    borderColor: '#2563eb',
  },

  roleOptionText: {
    color: '#344054',
    fontSize: 12,
    fontWeight: '700',
  },

  roleOptionTextSelected: {
    color: '#1d4ed8',
  },

  saveButton: {
    backgroundColor: '#16a34a',
    padding: 13,
    borderRadius: 9,
    alignItems: 'center',
  },

  saveButtonText: {
    color: '#fff',
    fontWeight: '800',
  },

  disabled: {
    opacity: 0.6,
  },

>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
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