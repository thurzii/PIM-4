import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';

// Configure EXPO_PUBLIC_API_URL in mobile/.env.
// Android emulator: http://10.0.2.2:3000/api
// Physical phone: http://IP_DO_PC:3000/api (same Wi-Fi as the computer)
export const API_URL =
  process.env.EXPO_PUBLIC_API_URL || 'http://10.0.2.2:3000/api';

const api = axios.create({
  baseURL: API_URL,
  timeout: 15000,
});

api.interceptors.request.use(async config => {
  const token = await AsyncStorage.getItem('@cursos:token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export default api;
