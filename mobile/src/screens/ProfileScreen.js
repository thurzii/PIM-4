<<<<<<< HEAD
import React from 'react';

import {
  Alert,
  Pressable,
=======
import React, {
  useCallback,
  useState,
} from 'react';

import {
  ActivityIndicator,
  Alert,
  Pressable,
  RefreshControl,
  ScrollView,
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  StyleSheet,
  Text,
  View,
} from 'react-native';

<<<<<<< HEAD
import { useAuth } from '../context/AuthContext';

export default function ProfileScreen() {
  const { user, logout } = useAuth();
=======
import { useFocusEffect } from '@react-navigation/native';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';

export default function ProfileScreen() {
  const { user, logout } = useAuth();
  const [gamification, setGamification] = useState(null);
  const [loadingGamification, setLoadingGamification] = useState(false);

  const loadGamification = useCallback(async () => {
    if (user?.perfil !== 'ALUNO') return;

    setLoadingGamification(true);
    try {
      const response = await api.get('/gamificacao/me');
      setGamification(response.data);
    } catch (error) {
      Alert.alert(
        'Gamificação indisponível',
        error.response?.data?.message ||
          'Não foi possível carregar seus pontos e conquistas.'
      );
    } finally {
      setLoadingGamification(false);
    }
  }, [user?.perfil]);

  useFocusEffect(
    useCallback(() => {
      loadGamification();
    }, [loadGamification])
  );
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)

  function sair() {
    Alert.alert(
      'Sair',
      'Deseja encerrar a sessão?',
      [
<<<<<<< HEAD
        {
          text: 'Cancelar',
        },
=======
        { text: 'Cancelar' },
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
        {
          text: 'Sair',
          style: 'destructive',
          onPress: logout,
        },
      ]
    );
  }

<<<<<<< HEAD
  return (
    <View style={styles.container}>
=======
  const progressoNivel = gamification
    ? Math.min(100, Math.max(0, gamification.progressoNivel))
    : 0;

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
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
      <View style={styles.avatar}>
        <Text style={styles.avatarText}>
          {user?.nome?.charAt(0)?.toUpperCase()}
        </Text>
      </View>

<<<<<<< HEAD
      <Text style={styles.name}>
        {user?.nome}
      </Text>

      <Text style={styles.email}>
        {user?.email}
      </Text>

      <View style={styles.card}>
        <Text>
          Perfil
        </Text>

        <Text style={styles.bold}>
          {user?.perfil === 'ADMIN'
            ? 'Administrador'
=======
      <Text style={styles.name}>{user?.nome}</Text>
      <Text style={styles.email}>{user?.email}</Text>    
      {user?.modoDemo && (      
        <View style={styles.demoBadge}>         
        <Text style={styles.demoBadgeText}>MODO DE DEMONSTRAÇÃO — dados temporários</Text>     
        </View>    
      )}

      <View style={styles.card}>
        <Text style={styles.cardLabel}>Perfil</Text>
        <Text style={styles.bold}>
          {user?.perfil === 'ADMIN'
            ? 'Administrador'
            : user?.perfil === 'PROFESSOR'
            ? 'Professor'
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
            : 'Aluno'}
        </Text>
      </View>

<<<<<<< HEAD
      <Pressable
        style={styles.logoutButton}
        onPress={sair}
      >
        <Text style={styles.logoutText}>
          Encerrar sessão
        </Text>
      </Pressable>
    </View>
=======
      {user?.perfil === 'ALUNO' && (
        <View style={styles.gamificationCard}>
          <Text style={styles.sectionTitle}>Sua jornada</Text>

          {loadingGamification && !gamification ? (
            <ActivityIndicator size="large" color="#2563eb" />
          ) : gamification ? (
            <>
              <View style={styles.statsRow}>
                <View style={styles.statBox}>
                  <Text style={styles.statValue}>{gamification.xp}</Text>
                  <Text style={styles.statLabel}>XP total</Text>
                </View>
                <View style={styles.statBox}>
                  <Text style={styles.statValue}>Nv. {gamification.nivel}</Text>
                  <Text style={styles.statLabel}>Nível atual</Text>
                </View>
              </View>

              <View style={styles.levelHeader}>
                <Text style={styles.levelText}>Progresso do nível</Text>
                <Text style={styles.levelText}>
                  {gamification.xpNoNivel}/100 XP
                </Text>
              </View>
              <View style={styles.progressTrack}>
                <View
                  style={[
                    styles.progressFill,
                    { width: `${progressoNivel}%` },
                  ]}
                />
              </View>

              <View style={styles.streakRow}>
                <Text style={styles.streakIcon}>🔥</Text>
                <View style={styles.streakInfo}>
                  <Text style={styles.streakTitle}>
                    {gamification.sequenciaAtual} dias de sequência
                  </Text>
                  <Text style={styles.streakSubtitle}>
                    Melhor sequência: {gamification.melhorSequencia} dias
                  </Text>
                </View>
                <Text style={styles.streakIcon}>🏆</Text>
              </View>

              <Text style={styles.sectionSubtitle}>
                Conquistas ({gamification.conquistas.filter((item) => item.conquistada).length}/{gamification.conquistas.length})
              </Text>

              <View style={styles.badgesGrid}>
                {gamification.conquistas.map((conquista) => (
                  <View
                    key={conquista.id}
                    style={[
                      styles.badgeItem,
                      !conquista.conquistada && styles.badgeLocked,
                    ]}
                  >
                    <Text style={styles.badgeIcon}>
                      {conquista.conquistada ? conquista.icone : '🔒'}
                    </Text>
                    <Text style={styles.badgeName}>
                      {conquista.nome}
                    </Text>
                    <Text style={styles.badgeDescription}>
                      {conquista.descricao}
                    </Text>
                    <Text style={styles.badgeXp}>
                      +{conquista.xpBonus} XP
                    </Text>
                  </View>
                ))}
              </View>
            </>
          ) : (
            <Text style={styles.emptyText}>
              Conclua uma aula para começar a ganhar XP e desbloquear conquistas.
            </Text>
          )}
        </View>
      )}

      <Pressable style={styles.logoutButton} onPress={sair}>
        <Text style={styles.logoutText}>Encerrar sessão</Text>
      </Pressable>
    </ScrollView>
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f4f6f8',
<<<<<<< HEAD
    alignItems: 'center',
    padding: 25,
  },

=======
  },
  content: {
    alignItems: 'center',
    padding: 25,
    paddingBottom: 40,
  },
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  avatar: {
    width: 90,
    height: 90,
    borderRadius: 45,
    backgroundColor: '#2563eb',
    alignItems: 'center',
    justifyContent: 'center',
    marginTop: 30,
  },
<<<<<<< HEAD

=======
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  avatarText: {
    color: '#fff',
    fontSize: 36,
    fontWeight: '800',
  },
<<<<<<< HEAD

=======
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  name: {
    fontSize: 23,
    fontWeight: '800',
    marginTop: 15,
  },
<<<<<<< HEAD

=======
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  email: {
    color: '#667085',
    marginTop: 5,
  },
<<<<<<< HEAD

=======
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  card: {
    width: '100%',
    backgroundColor: '#fff',
    padding: 18,
    borderRadius: 12,
<<<<<<< HEAD
    marginTop: 30,
  },

=======
    marginTop: 25,
  },
  cardLabel: {
    color: '#667085',
  },
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  bold: {
    fontWeight: '700',
    marginTop: 7,
  },
<<<<<<< HEAD

=======
  gamificationCard: {
    width: '100%',
    backgroundColor: '#fff',
    padding: 18,
    borderRadius: 14,
    marginTop: 16,
  },
  sectionTitle: {
    fontSize: 20,
    fontWeight: '800',
    marginBottom: 16,
    color: '#172033',
  },
  statsRow: {
    flexDirection: 'row',
    gap: 12,
  },
  statBox: {
    flex: 1,
    backgroundColor: '#eff6ff',
    borderRadius: 12,
    padding: 14,
  },
  statValue: {
    fontSize: 23,
    fontWeight: '800',
    color: '#1d4ed8',
  },
  statLabel: {
    color: '#64748b',
    marginTop: 4,
    fontSize: 12,
  },
  levelHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginTop: 18,
    marginBottom: 8,
  },
  levelText: {
    color: '#475569',
    fontSize: 12,
    fontWeight: '600',
  },
  progressTrack: {
    height: 10,
    borderRadius: 5,
    backgroundColor: '#e2e8f0',
    overflow: 'hidden',
  },
  progressFill: {
    height: '100%',
    backgroundColor: '#2563eb',
    borderRadius: 5,
  },
  streakRow: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#fff7ed',
    borderRadius: 12,
    padding: 13,
    marginTop: 18,
  },
  streakIcon: {
    fontSize: 25,
  },
  streakInfo: {
    flex: 1,
    marginLeft: 10,
  },
  streakTitle: {
    fontWeight: '800',
    color: '#9a3412',
  },
  streakSubtitle: {
    color: '#9a3412',
    fontSize: 12,
    marginTop: 3,
  },
  sectionSubtitle: {
    fontSize: 16,
    fontWeight: '800',
    marginTop: 22,
    marginBottom: 12,
  },
  badgesGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: 10,
  },
  badgeItem: {
    width: '48%',
    flexGrow: 1,
    backgroundColor: '#f0fdf4',
    borderColor: '#bbf7d0',
    borderWidth: 1,
    borderRadius: 12,
    padding: 11,
  },
  badgeLocked: {
    backgroundColor: '#f8fafc',
    borderColor: '#e2e8f0',
    opacity: 0.72,
  },
  badgeIcon: {
    fontSize: 25,
    marginBottom: 7,
  },
  badgeName: {
    fontWeight: '800',
    color: '#1e293b',
    fontSize: 13,
  },
  badgeDescription: {
    color: '#64748b',
    fontSize: 11,
    marginTop: 4,
    minHeight: 30,
  },
  badgeXp: {
    color: '#15803d',
    fontWeight: '700',
    fontSize: 11,
    marginTop: 7,
  },
  emptyText: {
    color: '#64748b',
    lineHeight: 21,
  },
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  logoutButton: {
    width: '100%',
    padding: 15,
    borderRadius: 10,
    backgroundColor: '#fff',
    borderWidth: 1,
    borderColor: '#ef4444',
    alignItems: 'center',
<<<<<<< HEAD
    marginTop: 15,
  },

=======
    marginTop: 16,
  },
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
  logoutText: {
    color: '#dc2626',
    fontWeight: '700',
  },
<<<<<<< HEAD
});
=======
});
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
