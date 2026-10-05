import React, { createContext, useContext, useEffect, useState } from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import api from '../services/api';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => { restore(); }, []);

  async function restore() {
    try { const saved = await AsyncStorage.getItem('@cursos:user'); if (saved) setUser(JSON.parse(saved)); }
    finally { setLoading(false); }
  }

  async function login(email, senha) {
    const { data } = await api.post('/auth/login', { email, senha });
    await AsyncStorage.multiSet([['@cursos:token', data.token], ['@cursos:user', JSON.stringify(data.user)]]);
    setUser(data.user);
  }

  async function logout() {
    await AsyncStorage.multiRemove(['@cursos:token', '@cursos:user']);
    setUser(null);
  }

  return <AuthContext.Provider value={{ user, loading, login, logout }}>{children}</AuthContext.Provider>;
}

export const useAuth = () => useContext(AuthContext);
