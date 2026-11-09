import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';

// TROQUE PELO IP DO COMPUTADOR QUE EXECUTA A API.
// Celular fisico: http://IP_DO_PC:3000/api
// Android Emulator: http://10.0.2.2:3000/api
export const API_URL = 'http://10.0.2.2:3000/api';

const api = axios.create({ baseURL: API_URL, timeout: 10000 });

api.interceptors.request.use(async config => {
  const token = await AsyncStorage.getItem('@cursos:token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

<<<<<<< HEAD
=======
async function request(method, url, config = {}) {
  if (isDemoMode()) {
    const mocked = await demoRequest(method, url, config);
    if (mocked) return mocked;
  }
  return client.request({ method, url, ...config });
}

const api = {
  get: (url, config) => request('get', url, config),
  post: (url, data, config = {}) => request('post', url, { ...config, data }),
  put: (url, data, config = {}) => request('put', url, { ...config, data }),
  delete: (url, config) => request('delete', url, config),
};
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
export default api;
