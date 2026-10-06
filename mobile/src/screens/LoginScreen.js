import React, { useState } from 'react';

import {
  Alert,
  KeyboardAvoidingView,
  Platform,
  Pressable,
  StyleSheet,
  Text,
  TextInput,
  View,
} from 'react-native';

import { useAuth } from '../context/AuthContext';

export default function LoginScreen({ navigation }) {
  const { login } = useAuth();

  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [carregando, setCarregando] = useState(false);

  async function entrar() {
    if (!email || !senha) {
      Alert.alert(
        'Atenção',
        'Informe email e senha.'
      );

      return;
    }

    setCarregando(true);

    try {
      await login(email, senha);
    } catch (error) {
      Alert.alert(
        'Login',
        error.response?.data?.message ||
          'Não foi possível conectar à API.'
      );
    } finally {
      setCarregando(false);
    }
  }

  return (
    <KeyboardAvoidingView
      style={styles.container}
      behavior={
        Platform.OS === 'ios'
          ? 'padding'
          : undefined
      }
    >
      <View style={styles.box}>
        <Text style={styles.logo}>
          PLATAFORMA
        </Text>

        <Text style={styles.subtitle}>
          Cursos Online
        </Text>

        <TextInput
          style={styles.input}
          placeholder="Email"
          autoCapitalize="none"
          keyboardType="email-address"
          value={email}
          onChangeText={setEmail}
        />

        <TextInput
          style={styles.input}
          placeholder="Senha"
          secureTextEntry
          value={senha}
          onChangeText={setSenha}
        />

        <Pressable
          style={styles.button}
          onPress={entrar}
          disabled={carregando}
        >
          <Text style={styles.buttonText}>
            {carregando ? 'Entrando...' : 'Entrar'}
          </Text>
        </Pressable>

        <Pressable
          onPress={() =>
            navigation.navigate('RecuperarSenha')
          }
        >
          <Text style={styles.link}>
            Esqueci minha senha
          </Text>
        </Pressable>

        <Text style={styles.demo}>
          Demo: maria@escola.com / aluno123
        </Text>
      </View>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f4f6f8',
    justifyContent: 'center',
    padding: 24,
  },

  box: {
    backgroundColor: '#fff',
    padding: 24,
    borderRadius: 18,
  },

  logo: {
    fontSize: 28,
    fontWeight: '800',
    textAlign: 'center',
  },

  subtitle: {
    textAlign: 'center',
    color: '#667085',
    marginBottom: 28,
  },

  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 10,
    padding: 14,
    marginBottom: 12,
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

  link: {
    textAlign: 'center',
    marginTop: 18,
    color: '#2563eb',
  },

  demo: {
    fontSize: 11,
    color: '#888',
    textAlign: 'center',
    marginTop: 25,
  },
});