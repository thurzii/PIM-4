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
  const [mostrarSenha, setMostrarSenha] = useState(false);
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
          keyboardType="email-address"
          autoCapitalize="none"
          value={email}
          onChangeText={setEmail}
        />

        <View style={styles.passwordContainer}>
          <TextInput
            style={styles.passwordInput}
            placeholder="Senha"
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
    width: '100%',
  },

  logo: {
    fontSize: 30,
    fontWeight: '800',
    textAlign: 'center',
  },

  subtitle: {
    fontSize: 18,
    color: '#667085',
    textAlign: 'center',
    marginTop: 5,
    marginBottom: 30,
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

  link: {
    color: '#2563eb',
    textAlign: 'center',
    marginTop: 18,
    fontWeight: '600',
  },

  demo: {
    color: '#667085',
    textAlign: 'center',
    marginTop: 25,
    fontSize: 12,
  },
});