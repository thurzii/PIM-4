import React, {
  useCallback,
  useState,
} from 'react';

import {
  Alert,
  Pressable,
  Modal,
  ScrollView,
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
  const [modal, setModal] = useState(false);
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [perfil, setPerfil] = useState('ALUNO');
  const [saving, setSaving] = useState(false);
  async function criarUsuario() {
    if (!nome.trim() || !email.trim() || !senha) return Alert.alert('Cadastro', 'Preencha todos os campos.');
    setSaving(true);
    try {
      await api.post('/admin/usuarios', { nome: nome.trim(), email: email.trim(), senha, perfil });
      setModal(false); setNome(''); setEmail(''); setSenha(''); setPerfil('ALUNO');
      await load(busca);
      Alert.alert('Sucesso', 'Usuário cadastrado.');
    } catch (error) { Alert.alert('Cadastro', error.response?.data?.message || 'Erro ao cadastrar usuário.'); }
    finally { setSaving(false); }
  }


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

      <Pressable style={styles.addButton} onPress={() => setModal(true)}><Text style={styles.addText}>+ Criar usuário</Text></Pressable>
      <Modal visible={modal} animationType="slide" onRequestClose={() => setModal(false)}>
        <ScrollView contentContainerStyle={styles.modalContent} keyboardShouldPersistTaps="handled">
          <Text style={styles.title}>Novo usuário</Text>
          <TextInput style={styles.search} placeholder="Nome completo" value={nome} onChangeText={setNome} />
          <TextInput style={styles.search} placeholder="E-mail" value={email} onChangeText={setEmail} autoCapitalize="none" keyboardType="email-address" />
          <TextInput style={styles.search} placeholder="Senha provisória (mínimo 8 caracteres)" value={senha} onChangeText={setSenha} secureTextEntry />
          <Text style={styles.roleLabel}>Perfil</Text>
          <View style={styles.roles}>{['ALUNO', 'PROFESSOR', 'ADMIN'].map(value =>
            <Pressable key={value} style={[styles.roleButton, perfil === value && styles.selectedRole]} onPress={() => setPerfil(value)}>
              <Text style={perfil === value ? styles.selectedText : styles.roleText}>{value}</Text>
            </Pressable>)}</View>
          <Pressable style={styles.addButton} onPress={criarUsuario} disabled={saving}><Text style={styles.addText}>{saving ? 'Salvando...' : 'Cadastrar'}</Text></Pressable>
          <Pressable onPress={() => setModal(false)}><Text style={styles.cancel}>Cancelar</Text></Pressable>
        </ScrollView>
      </Modal>
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

  modalContent: { padding: 22, paddingTop: 55 },
  addButton: { backgroundColor: '#2563eb', padding: 14, borderRadius: 10, alignItems: 'center', marginTop: 12 },
  addText: { color: '#fff', fontWeight: '700' },
  roles: { flexDirection: 'row', gap: 6, marginVertical: 12 },
  roleButton: { flex: 1, padding: 10, borderRadius: 8, backgroundColor: '#e5e7eb', alignItems: 'center' },
  selectedRole: { backgroundColor: '#2563eb' },
  selectedText: { color: '#fff', fontWeight: '700', fontSize: 11 },
  roleText: { color: '#344054', fontSize: 11 },
  roleLabel: { marginTop: 10, fontWeight: '700' },
  cancel: { textAlign: 'center', color: '#2563eb', padding: 20 },
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