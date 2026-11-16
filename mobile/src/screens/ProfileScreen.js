import React, { useCallback, useEffect, useState } from 'react';

import {
  ActivityIndicator,
  Alert,
  Pressable,
  RefreshControl,
  ScrollView,
  StyleSheet,
  Text,
  View,
} from 'react-native';

import { useAuth } from '../context/AuthContext';
import api from '../services/api';

export default function ProfileScreen() {
  const { user, logout } = useAuth();
  const [gamification, setGamification] = useState(null);
  const [loadingGamification, setLoadingGamification] = useState(false);
  const [gamificationError, setGamificationError] = useState('');

  const isStudent = user?.perfil === 'ALUNO';

  const loadGamification = useCallback(async () => {
    if (!isStudent) {
      setGamification(null);
      setGamificationError('');
      return;
    }

    setLoadingGamification(true);
    setGamificationError('');

    try {
      const { data } = await api.get('/gamificacao/me');
      setGamification(data || null);
    } catch (error) {
      setGamification(null);
      setGamificationError(
        (error?.response?.status === 404 ? 'Reinicie a API atualizada para habilitar suas conquistas.' : error?.response?.data?.message) ||
        'Não foi possível carregar suas conquistas.'
      );
    } finally {
      setLoadingGamification(false);
    }
  }, [isStudent]);

  useEffect(() => {
    loadGamification();
  }, [loadGamification]);

  function sair() {
    Alert.alert(
      'Sair',
      'Deseja encerrar a sessão?',
      [
        { text: 'Cancelar', style: 'cancel' },
        {
          text: 'Sair',
          style: 'destructive',
          onPress: logout,
        },
      ]
    );
  }

  const progressoNivel = gamification
    ? Math.min(100, Math.max(0, Number(gamification.progressoNivel || 0)))
    : 0;

  const perfilLabel = {
    ADMIN: 'Administrador',
    PROFESSOR: 'Professor',
    ALUNO: 'Aluno',
  }[user?.perfil] || user?.perfil || 'Não informado';

  return (
    <ScrollView
      style={styles.container}
      contentContainerStyle={styles.content}
      refreshControl={
        <RefreshControl
          refreshing={loadingGamification}
          onRefresh={loadGamification}
        />
      }
    >
      <View style={styles.avatar}>
        <Text style={styles.avatarText}>
          {user?.nome?.charAt(0)?.toUpperCase() || '?'}
        </Text>
      </View>

      <Text style={styles.name}>{user?.nome || 'Usuário'}</Text>
      <Text style={styles.email}>{user?.email || ''}</Text>

      <View style={styles.card}>
        <Text style={styles.cardLabel}>Perfil</Text>
        <Text style={styles.bold}>{perfilLabel}</Text>
      </View>

      {isStudent && (
        <View style={styles.card}>
          <Text style={styles.sectionTitle}>Suas conquistas</Text>

          {loadingGamification && !gamification ? (
            <ActivityIndicator style={styles.loader} />
          ) : gamification ? (
            <>
              <View style={styles.statsRow}>
                <View style={styles.stat}>
                  <Text style={styles.statValue}>{gamification.xp ?? 0}</Text>
                  <Text style={styles.statLabel}>XP</Text>
                </View>
                <View style={styles.stat}>
                  <Text style={styles.statValue}>{gamification.nivel ?? 1}</Text>
                  <Text style={styles.statLabel}>Nível</Text>
                </View>
                <View style={styles.stat}>
                  <Text style={styles.statValue}>{gamification.aulasConcluidas ?? 0}</Text>
                  <Text style={styles.statLabel}>Aulas concluídas</Text>
                </View>
              </View>

              <Text style={styles.progressLabel}>
                Progresso do nível: {progressoNivel}%
              </Text>
              <View style={styles.progressTrack}>
                <View style={[styles.progressFill, { width: `${progressoNivel}%` }]} />
              </View>

              <Text style={styles.streakText}>
                Sequência atual: {gamification.sequenciaAtual ?? 0} dia(s)
              </Text>
              <Text style={styles.streakText}>
                Melhor sequência: {gamification.melhorSequencia ?? 0} dia(s)
              </Text>

              {Array.isArray(gamification.conquistas) && gamification.conquistas.length > 0 && (
                <View style={styles.achievements}>
                  <Text style={styles.achievementTitle}>Conquistas</Text>
                  {gamification.conquistas.map(item => (
                    <View key={String(item.id)} style={styles.achievementRow}>
                      <Text style={styles.achievementName}>
                        {item.icone ? `${item.icone} ` : ''}{item.nome}
                      </Text>
                      <Text style={styles.achievementStatus}>
                        {item.conquistada ? 'Conquistada' : 'Bloqueada'}
                      </Text>
                    </View>
                  ))}
                </View>
              )}
            </>
          ) : (
            <Text style={styles.helperText}>
              {gamificationError || 'Ainda não há dados de gamificação disponíveis.'}
            </Text>
          )}
        </View>
      )}

      <Pressable style={styles.logoutButton} onPress={sair}>
        <Text style={styles.logoutText}>Encerrar sessão</Text>
      </Pressable>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f4f6f8',
  },
  content: {
    alignItems: 'center',
    padding: 25,
    paddingBottom: 40,
  },
  avatar: {
    width: 90,
    height: 90,
    borderRadius: 45,
    backgroundColor: '#2563eb',
    alignItems: 'center',
    justifyContent: 'center',
    marginTop: 30,
  },
  avatarText: {
    color: '#fff',
    fontSize: 36,
    fontWeight: '800',
  },
  name: {
    fontSize: 23,
    fontWeight: '800',
    marginTop: 15,
  },
  email: {
    color: '#667085',
    marginTop: 5,
  },
  card: {
    width: '100%',
    backgroundColor: '#fff',
    padding: 18,
    borderRadius: 12,
    marginTop: 20,
  },
  cardLabel: {
    color: '#667085',
    fontSize: 13,
  },
  bold: {
    fontWeight: '700',
    marginTop: 7,
  },
  sectionTitle: {
    fontSize: 17,
    fontWeight: '800',
    marginBottom: 15,
  },
  loader: {
    padding: 18,
  },
  statsRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  stat: {
    flex: 1,
    alignItems: 'center',
  },
  statValue: {
    color: '#2563eb',
    fontSize: 22,
    fontWeight: '800',
  },
  statLabel: {
    color: '#667085',
    fontSize: 11,
    marginTop: 4,
    textAlign: 'center',
  },
  progressLabel: {
    color: '#475467',
    fontSize: 12,
    marginTop: 20,
    marginBottom: 7,
  },
  progressTrack: {
    height: 9,
    backgroundColor: '#e5e7eb',
    borderRadius: 8,
    overflow: 'hidden',
  },
  progressFill: {
    height: '100%',
    backgroundColor: '#2563eb',
    borderRadius: 8,
  },
  streakText: {
    color: '#475467',
    fontSize: 12,
    marginTop: 9,
  },
  achievements: {
    marginTop: 20,
  },
  achievementTitle: {
    fontWeight: '800',
    marginBottom: 8,
  },
  achievementRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingVertical: 8,
    borderTopWidth: 1,
    borderTopColor: '#eef0f2',
  },
  achievementName: {
    flex: 1,
    color: '#344054',
  },
  achievementStatus: {
    color: '#667085',
    fontSize: 11,
    marginLeft: 8,
  },
  helperText: {
    color: '#667085',
    lineHeight: 20,
  },
  logoutButton: {
    width: '100%',
    padding: 15,
    borderRadius: 10,
    backgroundColor: '#fff',
    borderWidth: 1,
    borderColor: '#ef4444',
    alignItems: 'center',
    marginTop: 20,
  },
  logoutText: {
    color: '#dc2626',
    fontWeight: '700',
  },
});
