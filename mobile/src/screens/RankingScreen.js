import React, { useCallback, useState } from 'react';
import { ActivityIndicator, FlatList, RefreshControl, StyleSheet, Text, View } from 'react-native';
import { useFocusEffect } from '@react-navigation/native';
import api from '../services/api';

export default function RankingScreen() {
  const [alunos, setAlunos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState('');
  const carregar = useCallback(async () => {
    setLoading(true);
    try {
      const { data } = await api.get('/gamificacao/turma');
      setAlunos(Array.isArray(data) ? data : []);
      setErro('');
    } catch (e) {
      setErro(e.response?.data?.message || 'Não foi possível carregar o ranking.');
    } finally { setLoading(false); }
  }, []);
  useFocusEffect(useCallback(() => { carregar(); }, [carregar]));
  return <View style={styles.container}>
    <Text style={styles.title}>Ranking de XP</Text>
    <Text style={styles.subtitle}>Classificação geral dos alunos ativos</Text>
    {loading && !alunos.length ? <ActivityIndicator size="large" /> : null}
    {!!erro && <Text style={styles.error}>{erro}</Text>}
    <FlatList data={alunos} keyExtractor={item => String(item.id)}
      refreshControl={<RefreshControl refreshing={loading} onRefresh={carregar} />}
      ListEmptyComponent={!loading && !erro ? <Text>Nenhum aluno encontrado.</Text> : null}
      renderItem={({ item }) => <View style={styles.card}>
        <Text style={styles.position}>#{item.posicao}</Text>
        <View style={{ flex: 1 }}><Text style={styles.name}>{item.nome}</Text>
          <Text style={styles.detail}>Nível {item.nivel} · Sequência {item.sequenciaAtual || 0}</Text></View>
        <Text style={styles.xp}>{item.xp} XP</Text>
      </View>} />
  </View>;
}
const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#F4F6F8', padding: 18 },
  title: { fontSize: 25, fontWeight: '800', color: '#172033' },
  subtitle: { color: '#667085', marginBottom: 18 },
  error: { color: '#B42318', marginBottom: 12 },
  card: { flexDirection: 'row', alignItems: 'center', gap: 12, padding: 16, backgroundColor: '#FFF', borderRadius: 12, marginBottom: 10 },
  position: { fontWeight: '800', fontSize: 17, color: '#4F46E5', width: 42 },
  name: { fontWeight: '700', color: '#172033' },
  detail: { color: '#667085', marginTop: 4 },
  xp: { fontWeight: '800', color: '#4F46E5' },
});
