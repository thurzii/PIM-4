import React from 'react';

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
<<<<<<< HEAD
=======
import ProfessorCoursesScreen from '../screens/ProfessorCoursesScreen';
import ProfessorCourseScreen from '../screens/ProfessorCourseScreen';
import ProfessorStudentsScreen from '../screens/ProfessorStudentsScreen';
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)

const Stack = createNativeStackNavigator();
const Tabs = createBottomTabNavigator();

function StudentTabs() {
  return (
    <Tabs.Navigator
      screenOptions={{
        headerShown: false,
      }}
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

<<<<<<< HEAD
=======
function ProfessorTabs() {
  return (
    <Tabs.Navigator
      screenOptions={{
        headerShown: false,
      }}
    >
      <Tabs.Screen
        name="Meus cursos"
        component={ProfessorCoursesScreen}
      />

      <Tabs.Screen
        name="Perfil"
        component={ProfileScreen}
      />
    </Tabs.Navigator>
  );
}

>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
function AdminTabs() {
  return (
    <Tabs.Navigator
      screenOptions={{
        headerShown: false,
      }}
    >
      <Tabs.Screen
        name="Admin"
        component={AdminScreen}
      />

      <Tabs.Screen
        name="Usuários"
        component={AdminUsersScreen}
      />

      <Tabs.Screen
        name="Perfil"
        component={ProfileScreen}
      />
    </Tabs.Navigator>
  );
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
<<<<<<< HEAD
=======
        ) : user.perfil === 'PROFESSOR' ? (
          <>
            <Stack.Screen
              name="Principal"
              component={ProfessorTabs}
              options={{
                headerShown: false,
              }}
            />

            <Stack.Screen
              name="ProfessorCurso"
              component={ProfessorCourseScreen}
              options={{
                title: 'Gerenciar curso',
              }}
            />

            <Stack.Screen
              name="ProfessorAlunos"
              component={ProfessorStudentsScreen}
              options={{
                title: 'Alunos do curso',
              }}
            />
          </>
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
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