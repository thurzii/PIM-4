<<<<<<< HEAD
import React, {
  createContext,
  useContext,
  useEffect,
  useState,
} from 'react';

import AsyncStorage from '@react-native-async-storage/async-storage';

import api from '../services/api';
=======
import React, { createContext, useContext, useEffect, useState } from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import api from '../services/api';
import { demoLogin, setDemoMode } from '../services/demo';
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

<<<<<<< HEAD
  useEffect(() => {
    restore();
  }, []);

  async function restore() {
    try {
      const savedUser = await AsyncStorage.getItem(
        '@cursos:user'
      );

      if (savedUser) {
        setUser(JSON.parse(savedUser));
      }
    } finally {
      setLoading(false);
    }
  }

  async function login(email, senha) {
    const { data } = await api.post(
      '/auth/login',
      {
        email,
        senha,
      }
    );

    await AsyncStorage.multiSet([
      ['@cursos:token', data.token],
      ['@cursos:user', JSON.stringify(data.user)],
    ]);

=======
  useEffect(() => { restore(); }, []);

  async function restore() {
    try {
      const values = await AsyncStorage.multiGet(['@cursos:token', '@cursos:user', '@cursos:demo']);
      const token = values[0][1];
      const savedUser = values[1][1];
      const savedDemo = values[2][1] === 'true';
      if (token && savedUser) {
        const parsedUser = JSON.parse(savedUser);
        if (parsedUser?.id && parsedUser?.perfil) {
          if (savedDemo) setDemoMode(true);
          setUser(parsedUser);
        } else await clearSession();
      } else await clearSession();
    } catch (error) {
      console.warn('Não foi possível restaurar a sessão:', error.message);
      setUser(null);
    } finally { setLoading(false); }
  }

  async function clearSession() {
    setDemoMode(false);
    await AsyncStorage.multiRemove(['@cursos:token', '@cursos:user', '@cursos:demo']);
  }

  async function login(email, senha) {
    const normalizedEmail = email.trim().toLowerCase();
    const demoAccounts = ['maria@escola.com', 'professor@escola.com', 'admin@escola.com'];
    if (demoAccounts.includes(normalizedEmail)) {
      const demoUser = demoLogin(normalizedEmail, senha);
      await AsyncStorage.multiSet([
        ['@cursos:token', 'demo-session'],
        ['@cursos:user', JSON.stringify(demoUser)],
        ['@cursos:demo', 'true'],
      ]);
      setUser(demoUser);
      return;
    }

    setDemoMode(false);
    const { data } = await api.post('/auth/login', { email: normalizedEmail, senha });
    if (!data.token || !data.user) throw new Error('A API não retornou os dados necessários para iniciar a sessão.');
    await AsyncStorage.multiSet([
      ['@cursos:token', data.token],
      ['@cursos:user', JSON.stringify(data.user)],
      ['@cursos:demo', 'false'],
    ]);
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
    setUser(data.user);
  }

  async function logout() {
<<<<<<< HEAD
    await AsyncStorage.multiRemove([
      '@cursos:token',
      '@cursos:user',
    ]);

    setUser(null);
  }

  return (
    <AuthContext.Provider
      value={{
        user,
        loading,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
=======
    await clearSession();
    setUser(null);
  }

  return <AuthContext.Provider value={{ user, loading, login, logout }}>{children}</AuthContext.Provider>;
}

export const useAuth = () => useContext(AuthContext);
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
