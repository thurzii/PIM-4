<<<<<<< HEAD
import React, { useState } from 'react';

import {
  Alert,
  Pressable,
  StyleSheet,
  Text,
  TextInput,
  View,
} from 'react-native';

import api from '../services/api';

export default function RecoverScreen({ navigation }) {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [mostrarSenha, setMostrarSenha] = useState(false);

  async function recuperar() {
    try {
      const response = await api.post(
        '/auth/recover',
        {
          email,
          novaSenha: senha,
        }
      );

      Alert.alert(
        'Sucesso',
        response.data.message,
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
          'Falha ao conectar.'
      );
    }
  }

=======
import React from 'react';

import {
  Pressable,
  StyleSheet,
  Text,
  View,
} from 'react-native';

export default function RecoverScreen({ navigation }) {
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  return (
    <View style={styles.container}>
      <Text style={styles.title}>
        Recuperar senha
      </Text>

<<<<<<< HEAD
      <Text style={styles.help}>
        Informe o email cadastrado e a nova senha.
      </Text>

      <TextInput
        style={styles.input}
        placeholder="Email"
        keyboardType="email-address"
        autoCapitalize="none"
        value={email}
        onChangeText={setEmail}
      />

      <View style={styles.passwordContainer}>
        <TextInput
          style={styles.passwordInput}
          placeholder="Nova senha"
          secureTextEntry={!mostrarSenha}
          value={senha}
          onChangeText={setSenha}
        />

        {senha.length > 0 && (
          <Pressable
            onPress={() =>
              setMostrarSenha(!mostrarSenha)
            }
          >
            <Text style={styles.showPassword}>
              {mostrarSenha ? 'Ocultar' : 'Mostrar'}
            </Text>
          </Pressable>
        )}
=======
      <View style={styles.notice}>
        <Text style={styles.noticeTitle}>
          Recuperação temporariamente indisponível
        </Text>

        <Text style={styles.noticeText}>
          Para proteger as contas, a redefinição não pode ser feita apenas informando o e-mail.
          Solicite ao administrador da plataforma que redefina sua senha.
        </Text>
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
      </View>

      <Pressable
        style={styles.button}
<<<<<<< HEAD
        onPress={recuperar}
      >
        <Text style={styles.buttonText}>
          Redefinir senha
=======
        onPress={() => navigation.goBack()}
      >
        <Text style={styles.buttonText}>
          Voltar para o login
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
        </Text>
      </Pressable>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 24,
    backgroundColor: '#f4f6f8',
  },

  title: {
    fontSize: 26,
    fontWeight: '800',
    marginTop: 30,
<<<<<<< HEAD
  },

  help: {
    color: '#667085',
    marginVertical: 18,
  },

  input: {
    backgroundColor: '#fff',
    borderWidth: 1,
    borderColor: '#ddd',
    padding: 14,
    borderRadius: 10,
    marginBottom: 12,
  },

  passwordContainer: {
    backgroundColor: '#fff',
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 10,
    marginBottom: 12,
    flexDirection: 'row',
    alignItems: 'center',
  },

  passwordInput: {
    flex: 1,
    padding: 14,
  },

  showPassword: {
    color: '#2563eb',
    fontWeight: '600',
    paddingHorizontal: 14,
=======
    marginBottom: 20,
  },

  notice: {
    backgroundColor: '#fff',
    borderWidth: 1,
    borderColor: '#e5e7eb',
    borderRadius: 12,
    padding: 16,
  },

  noticeTitle: {
    fontSize: 16,
    fontWeight: '700',
    color: '#111827',
    marginBottom: 8,
  },

  noticeText: {
    color: '#667085',
    lineHeight: 22,
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  },

  button: {
    backgroundColor: '#2563eb',
    padding: 15,
    borderRadius: 10,
    alignItems: 'center',
<<<<<<< HEAD
=======
    marginTop: 18,
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  },

  buttonText: {
    color: '#fff',
    fontWeight: '700',
  },
<<<<<<< HEAD
});
=======
});
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
