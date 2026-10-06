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

  return (
    <View style={styles.container}>
      <Text style={styles.title}>
        Recuperar senha
      </Text>

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
      </View>

      <Pressable
        style={styles.button}
        onPress={recuperar}
      >
        <Text style={styles.buttonText}>
          Redefinir senha
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
  },

  button: {
    backgroundColor: '#2563eb',
    padding: 15,
    borderRadius: 10,
    alignItems: 'center',
  },

  buttonText: {
    color: '#fff',
    fontWeight: '700',
  },
});