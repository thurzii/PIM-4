import React from 'react';
import { Pressable, StyleSheet, Text, View } from 'react-native';
export default function CourseCard({ course, onPress }) { return <Pressable style={s.card} onPress={onPress}><Text style={s.title}>{course.titulo}</Text><Text style={s.desc} numberOfLines={2}>{course.descricao || 'Sem descrição cadastrada.'}</Text><View style={s.row}><Text>{course.cargaHoraria}h</Text><Text>{course.totalAulas} aulas</Text></View></Pressable>; }
const s=StyleSheet.create({card:{backgroundColor:'#fff',padding:16,borderRadius:14,marginBottom:12,elevation:2},title:{fontSize:18,fontWeight:'700'},desc:{color:'#666',marginTop:6,lineHeight:20},row:{flexDirection:'row',justifyContent:'space-between',marginTop:12,color:'#555'}});
