import React from 'react';
import { Pressable, StyleSheet, Text, View } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
export default function CourseCard({ course, onPress }) {
  return <Pressable style={styles.card} onPress={onPress}>
    <View style={styles.row}><View style={styles.iconBox}><Ionicons name="book-outline" color="#5B50D8" size={25}/></View><View style={styles.badge}><Text style={styles.badgeText}>CURSO</Text></View></View>
    <Text style={styles.title}>{course.titulo}</Text>
    <Text style={styles.description} numberOfLines={2}>{course.descricao || 'Explore as aulas e amplie seus conhecimentos.'}</Text>
    <View style={styles.footer}><View style={styles.metadata}><Ionicons name="time-outline" size={15} color="#8792A5"/><Text style={styles.info}>{course.cargaHoraria ?? 0}h</Text><Ionicons name="layers-outline" size={15} color="#8792A5"/><Text style={styles.info}>{course.totalAulas ?? 0} aulas</Text></View><View style={styles.arrow}><Ionicons name="arrow-forward" size={17} color="#5146D9"/></View></View>
  </Pressable>;
}
const styles = StyleSheet.create({card: {backgroundColor:'#FFFFFF',padding:20,borderRadius:20,marginBottom:14,borderWidth:1,borderColor:'#EDF0F6'},row:{flexDirection:'row',alignItems:'center',justifyContent:'space-between',marginBottom:16},iconBox:{backgroundColor:'#EFEDFF',width:50,height:50,borderRadius:16,alignItems:'center',justifyContent:'center'},badge:{backgroundColor:'#F1F4F9',borderRadius:20,paddingHorizontal:10,paddingVertical:5},badgeText:{fontSize:10,color:'#758197',fontWeight:'800',letterSpacing:1},title:{fontSize:18,color:'#1F2B46',fontWeight:'800'},description:{color:'#8792A5',fontSize:13,lineHeight:20,marginTop:7},footer:{flexDirection:'row',alignItems:'center',justifyContent:'space-between',marginTop:18},metadata:{flexDirection:'row',alignItems:'center',gap:6},info:{color:'#8792A5',fontSize:12,marginRight:10},arrow:{width:32,height:32,borderRadius:11,backgroundColor:'#F0EEFF',alignItems:'center',justifyContent:'center'}});
