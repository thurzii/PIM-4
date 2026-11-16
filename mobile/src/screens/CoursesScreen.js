import React, { useCallback, useState } from 'react';
import { ActivityIndicator, FlatList, Pressable, RefreshControl, StyleSheet, Text, TextInput, View } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useFocusEffect } from '@react-navigation/native';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';
import CourseCard from '../components/CourseCard';

export default function CoursesScreen({ navigation }) {
  const { user } = useAuth();
  const [courses, setCourses] = useState([]);
  const [busca, setBusca] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const load = useCallback(async (term = '') => {
    setLoading(true); setError('');
    try {
      const response = await api.get('/cursos', { params: { busca: term } });
      setCourses(Array.isArray(response.data) ? response.data : []);
    } catch (e) {
      setError(e.response?.data?.message || 'Não foi possível carregar os cursos.');
    } finally { setLoading(false); }
  }, []);
  useFocusEffect(useCallback(() => { load(); }, [load]));
  const firstName = (user?.nome || 'Estudante').trim().split(' ')[0];
  return (
    <View style={styles.container}>
      <FlatList
        data={courses}
        keyExtractor={item => String(item.id)}
        contentContainerStyle={styles.content}
        keyboardShouldPersistTaps="handled"
        refreshControl={<RefreshControl refreshing={loading} onRefresh={() => load(busca)} />}
        ListHeaderComponent={<>
          <View style={styles.topline}><Text style={styles.brand}>APRENDA<Text style={styles.brandAccent}>+</Text></Text><Pressable onPress={() => navigation.navigate('Perfil')} style={styles.avatar}><Text style={styles.avatarText}>{firstName[0]?.toUpperCase()}</Text></Pressable></View>
          <Text style={styles.greeting}>Olá, {firstName}!</Text>
          <Text style={styles.subtitle}>Seu próximo aprendizado começa aqui.</Text>
          <View style={styles.hero}>
            <View style={styles.heroIcon}><Ionicons name="sparkles" size={22} color="#FFFFFF" /></View>
            <Text style={styles.heroEyebrow}>SEU ESPAÇO DE APRENDIZADO</Text>
            <Text style={styles.heroTitle}>Evolua no seu ritmo.</Text>
            <Text style={styles.heroDescription}>Explore novos conhecimentos e avance uma aula de cada vez.</Text>
            <View style={styles.heroPill}><Ionicons name="school-outline" size={15} color="#FFFFFF"/><Text style={styles.heroPillText}>{courses.length} {courses.length === 1 ? 'curso disponível' : 'cursos disponíveis'}</Text></View>
          </View>
          <View style={styles.sectionHeading}><View><Text style={styles.sectionTitle}>Explore os cursos</Text><Text style={styles.sectionSubtitle}>Escolha o que deseja aprender hoje</Text></View></View>
          <View style={styles.searchBox}><Ionicons name="search-outline" size={21} color="#8C96A8"/><TextInput style={styles.search} placeholder="Buscar cursos..." placeholderTextColor="#9AA3B2" value={busca} onChangeText={setBusca} onSubmitEditing={() => load(busca)} returnKeyType="search"/><Pressable onPress={() => load(busca)} accessibilityLabel="Pesquisar"><Ionicons name="arrow-forward-circle" size={25} color="#4F46E5"/></Pressable></View>
          {loading && <ActivityIndicator style={{ marginVertical: 22 }} color="#4F46E5" />}
          {!!error && <View style={styles.error}><Text style={styles.errorText}>{error}</Text><Pressable onPress={() => load(busca)}><Text style={styles.retry}>Tentar novamente</Text></Pressable></View>}
        </>}
        renderItem={({ item }) => <CourseCard course={item} onPress={() => navigation.navigate('CursoDetalhes', { id: item.id })} />}
        ListEmptyComponent={!loading && !error ? <View style={styles.empty}><Ionicons name="library-outline" size={36} color="#9AA3B2"/><Text style={styles.emptyTitle}>Nenhum curso encontrado</Text><Text style={styles.emptyDescription}>Experimente outra busca ou volte mais tarde.</Text></View> : null}
      />
    </View>
  );
}
const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#F6F8FC' }, content: { paddingHorizontal: 20, paddingTop: 22, paddingBottom: 38 },
  topline: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', marginBottom: 27 }, brand: { color: '#202947', fontWeight: '900', letterSpacing: 2, fontSize: 16 }, brandAccent: { color: '#635BEB' },
  avatar: { width: 42, height: 42, backgroundColor: '#E7E7FF', borderRadius: 15, alignItems: 'center', justifyContent: 'center' }, avatarText: { color: '#4F46E5', fontWeight: '800', fontSize: 17 },
  greeting: { fontSize: 29, fontWeight: '800', color: '#18233E' }, subtitle: { fontSize: 14, color: '#79849A', marginTop: 5, marginBottom: 23 },
  hero: { backgroundColor: '#5146D9', borderRadius: 25, padding: 24, marginBottom: 30, overflow: 'hidden' }, heroIcon: { backgroundColor: '#7067E7', width: 43, height: 43, borderRadius: 14, alignItems: 'center', justifyContent: 'center', marginBottom: 20 }, heroEyebrow: { color: '#DCD9FF', fontSize: 10, fontWeight: '800', letterSpacing: 1.4 }, heroTitle: { color: '#FFFFFF', fontSize: 26, fontWeight: '900', marginTop: 8 }, heroDescription: { color: '#E2E0FF', fontSize: 14, lineHeight: 22, marginTop: 8, maxWidth: 290 }, heroPill: { marginTop: 22, flexDirection: 'row', gap: 8, alignItems: 'center', alignSelf: 'flex-start', backgroundColor: '#6D64E5', borderRadius: 30, paddingVertical: 9, paddingHorizontal: 13 }, heroPillText: { color: '#FFFFFF', fontWeight: '700', fontSize: 12 },
  sectionHeading: { marginBottom: 16 }, sectionTitle: { color: '#1D2944', fontWeight: '800', fontSize: 21 }, sectionSubtitle: { color: '#8A94A6', marginTop: 4, fontSize: 13 },
  searchBox: { backgroundColor: '#FFFFFF', borderRadius: 16, paddingHorizontal: 15, height: 55, flexDirection: 'row', alignItems: 'center', gap: 10, marginBottom: 19, borderWidth: 1, borderColor: '#EDF0F6' }, search: { flex: 1, color: '#202947', fontSize: 14 },
  error: { backgroundColor: '#FFF1F2', borderRadius: 12, padding: 16, marginBottom: 14 }, errorText: { color: '#A33D48' }, retry: { color: '#4F46E5', fontWeight: '800', marginTop: 9 },
  empty: { alignItems: 'center', paddingVertical: 38 }, emptyTitle: { fontWeight: '800', color: '#344054', marginTop: 12 }, emptyDescription: { color: '#98A2B3', marginTop: 5, textAlign: 'center' },
});
