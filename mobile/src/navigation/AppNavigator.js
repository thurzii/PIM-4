import React from 'react';
import { Ionicons } from '@expo/vector-icons';

import {
  ActivityIndicator,
  View,
} from 'react-native';

import {
  NavigationContainer,
} from '@react-navigation/native';

import {
  createNativeStackNavigator,
} from '@react-navigation/native-stack';

import {
  createBottomTabNavigator,
} from '@react-navigation/bottom-tabs';

import { useAuth } from '../context/AuthContext';

import LoginScreen from '../screens/LoginScreen';
import RecoverScreen from '../screens/RecoverScreen';
import CoursesScreen from '../screens/CoursesScreen';
import CourseDetailsScreen from '../screens/CourseDetailsScreen';
import ProfileScreen from '../screens/ProfileScreen';
import AdminScreen from '../screens/AdminScreen';
import AdminCourseScreen from '../screens/AdminCourseScreen';
import AdminUsersScreen from '../screens/AdminUsersScreen';
import ProfessorCoursesScreen from '../screens/ProfessorCoursesScreen';
import ProfessorCourseScreen from '../screens/ProfessorCourseScreen';
import ProfessorStudentsScreen from '../screens/ProfessorStudentsScreen';
import RankingScreen from '../screens/RankingScreen';

const Stack = createNativeStackNavigator();
const Tabs = createBottomTabNavigator();

const tabIcons = { 'Cursos': ['home-outline', 'home'], 'Perfil': ['person-outline', 'person'], 'Admin': ['grid-outline', 'grid'], 'Usuários': ['people-outline', 'people'], 'Meus cursos': ['book-outline', 'book'], 'Ranking': ['trophy-outline', 'trophy'] };
const tabOptions = ({ route }) => ({
  headerShown: false,
  tabBarActiveTintColor: '#4F46E5',
  tabBarInactiveTintColor: '#8993A7',
  tabBarLabelStyle: { fontSize: 11, fontWeight: '700', marginBottom: 3 },
  tabBarStyle: { backgroundColor: '#FFFFFF', height: 66, paddingTop: 7, paddingBottom: 7, borderTopColor: '#E9EDF5' },
  tabBarIcon: ({ focused, color, size }) => {
    const icons = tabIcons[route.name] || ['ellipse-outline', 'ellipse'];
    return <Ionicons name={icons[focused ? 1 : 0]} size={23} color={color} />;
  },
});
function StudentTabs() {
  return (
    <Tabs.Navigator
      screenOptions={tabOptions}
    >
      <Tabs.Screen
        name="Cursos"
        component={CoursesScreen}
      />

      <Tabs.Screen
        name="Perfil"
        component={ProfileScreen}
      />
    </Tabs.Navigator>
  );
}

function AdminTabs() {
  return (
    <Tabs.Navigator
      screenOptions={tabOptions}
    >
      <Tabs.Screen
        name="Admin"
        component={AdminScreen}
      />

      <Tabs.Screen
        name="Usuários"
        component={AdminUsersScreen}
      />

      <Tabs.Screen name="Ranking" component={RankingScreen} />
      <Tabs.Screen name="Perfil" component={ProfileScreen} />
    </Tabs.Navigator>
  );
}

function ProfessorTabs() {
  return <Tabs.Navigator screenOptions={tabOptions}>
    <Tabs.Screen name="Meus cursos" component={ProfessorCoursesScreen} />
    <Tabs.Screen name="Ranking" component={RankingScreen} />
    <Tabs.Screen name="Perfil" component={ProfileScreen} />
  </Tabs.Navigator>;
}

export default function AppNavigator() {
  const { user, loading } = useAuth();

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" />
      </View>
    );
  }

  return (
    <NavigationContainer>
      <Stack.Navigator>
        {!user ? (
          <>
            <Stack.Screen
              name="Login"
              component={LoginScreen}
              options={{
                headerShown: false,
              }}
            />
            <Stack.Screen
              name="RecuperarSenha"
              component={RecoverScreen}
              options={{
                title: 'Recuperar senha',
              }}
            />
          </>
        ) : user.perfil === 'ADMIN' ? (
          <>
            <Stack.Screen
              name="Principal"
              component={AdminTabs}
              options={{
                headerShown: false,
              }}
            />

            <Stack.Screen
              name="AdminCurso"
              component={AdminCourseScreen}
              options={{
                title: 'Gerenciar curso',
              }}
            />
          </>
        ) : user.perfil === 'PROFESSOR' ? (
          <>
            <Stack.Screen name="Principal" component={ProfessorTabs} options={{ headerShown: false }} />
            <Stack.Screen name="ProfessorCurso" component={ProfessorCourseScreen} options={{ title: 'Gerenciar curso' }} />
            <Stack.Screen name="ProfessorAlunos" component={ProfessorStudentsScreen} options={{ title: 'Alunos matriculados' }} />
          </>
        ) : (
          <>
            <Stack.Screen
              name="Principal"
              component={StudentTabs}
              options={{
                headerShown: false,
              }}
            />

            <Stack.Screen
              name="CursoDetalhes"
              component={CourseDetailsScreen}
              options={{
                title: 'Curso',
              }}
            />
          </>
        )}
      </Stack.Navigator>
    </NavigationContainer>
  );
}

const styles = {
  loadingContainer: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
};