import React, {
  createContext,
  useContext,
  useEffect,
  useState,
} from 'react';

import AsyncStorage from '@react-native-async-storage/async-storage';

import api from '../services/api';

const AuthContext = createContext(null);

const TOKEN_KEY = '@cursos:token';
const USER_KEY = '@cursos:user';
const LEGACY_DEMO_KEY = '@cursos:demo';

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Por decisão do projeto, sempre exigir login ao abrir o app.
    async function clearPreviousSession() {
      try {
        await AsyncStorage.multiRemove([TOKEN_KEY, USER_KEY, LEGACY_DEMO_KEY]);
      } catch (error) {
        console.warn('Não foi possível limpar a sessão:', error);
      } finally {
        setUser(null);
        setLoading(false);
      }
    }
    clearPreviousSession();
  }, []);

  /* async function restore() {
    try {
      const storedValues = await AsyncStorage.multiGet([TOKEN_KEY, USER_KEY]);
      const token = storedValues[0]?.[1];
      const savedUser = storedValues[1]?.[1];

      let parsedUser = null;
      try {
        parsedUser = savedUser ? JSON.parse(savedUser) : null;
      } catch {
        parsedUser = null;
      }

      // Clear sessions created by the old local demonstration mode.
      const isDemoSession =
        !token ||
        token === 'demo-session' ||
        parsedUser?.modoDemo === true;

      await AsyncStorage.removeItem(LEGACY_DEMO_KEY);

      if (isDemoSession || !parsedUser) {
        await AsyncStorage.multiRemove([TOKEN_KEY, USER_KEY]);
        setUser(null);
        return;
      }

      setUser(parsedUser);
    } finally {
      setLoading(false);
    }
  }

  */

  async function login(email, senha) {
    const { data } = await api.post('/auth/login', {
      email: email.trim(),
      senha,
    });

    if (!data?.token || !data?.user) {
      throw new Error('A API não retornou uma sessão válida.');
    }

    await AsyncStorage.multiSet([
      [TOKEN_KEY, data.token],
      [USER_KEY, JSON.stringify(data.user)],
    ]);

    await AsyncStorage.removeItem(LEGACY_DEMO_KEY);
    setUser(data.user);
  }

  async function logout() {
    await AsyncStorage.multiRemove([TOKEN_KEY, USER_KEY, LEGACY_DEMO_KEY]);
    setUser(null);
  }

  return (
    <AuthContext.Provider value={{ user, loading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
