import React, { useState } from 'react';
import { Alert, ActivityIndicator, KeyboardAvoidingView, Platform, Pressable, ScrollView, StyleSheet, Text, TextInput, View } from 'react-native';
import api from '../services/api';

export default function RegisterScreen({ navigation }) {
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [confirmarSenha, setConfirmarSenha] = useState('');
  const [loading, setLoading] = useState(false);
  async function cadastrar() {
    if (!nome.trim() || !email.trim() || !senha || !confirmarSenha) return Alert.alert('Cadastro', 'Preencha todos os campos.');
    if (senha !== confirmarSenha) return Alert.alert('Cadastro', 'As senhas não conferem.');
    setLoading(true);
    try {
      await api.post('/auth/register', { nome: nome.trim(), email: email.trim(), senha, confirmarSenha });
      Alert.alert('Conta criada', 'Sua conta de aluno foi criada. Entre com seu e-mail e senha.', [ { text: 'Entrar', onPress: () => navigation.navigate('Login') } ]);
    } catch (error) {
      Alert.alert('Cadastro', error.response?.data?.message || 'Não foi possível criar sua conta.');
    } finally { setLoading(false); }
  }
  return <KeyboardAvoidingView style={styles.container} behavior={Platform.OS === 'ios' ? 'padding' : undefined}>
    <ScrollView contentContainerStyle={styles.content} keyboardShouldPersistTaps="handled">
      <Text style={styles.title}>Criar conta</Text><Text style={styles.subtitle}>Cadastro gratuito para alunos</Text>
      <TextInput style={styles.input} placeholder="Nome completo" value={nome} onChangeText={setNome} autoCapitalize="words" maxLength={200} />
      <TextInput style={styles.input} placeholder="E-mail" value={email} onChangeText={setEmail} autoCapitalize="none" keyboardType="email-address" maxLength={200} />
      <TextInput style={styles.input} placeholder="Senha (mínimo 8 caracteres)" value={senha} onChangeText={setSenha} secureTextEntry maxLength={128} />
      <TextInput style={styles.input} placeholder="Confirmar senha" value={confirmarSenha} onChangeText={setConfirmarSenha} secureTextEntry maxLength={128} />
      <Pressable style={styles.button} onPress={cadastrar} disabled={loading}>{loading ? <ActivityIndicator color="#fff"/> : <Text style={styles.buttonText}>Criar conta</Text>}</Pressable>
      <Pressable onPress={() => navigation.navigate('Login')}><Text style={styles.link}>Já tenho uma conta</Text></Pressable>
    </ScrollView>
  </KeyboardAvoidingView>;
}
const styles = StyleSheet.create({ container:{flex:1,backgroundColor:'#f4f6f8'},content:{flexGrow:1,justifyContent:'center',padding:24},title:{fontSize:28,fontWeight:'800',textAlign:'center'},subtitle:{textAlign:'center',color:'#667085',marginTop:6,marginBottom:28},input:{backgroundColor:'#fff',borderWidth:1,borderColor:'#ddd',padding:14,borderRadius:10,marginBottom:12},button:{backgroundColor:'#2563eb',padding:16,borderRadius:10,alignItems:'center'},buttonText:{color:'#fff',fontWeight:'700'},link:{color:'#2563eb',textAlign:'center',marginTop:20,fontWeight:'600'} });
