import React from 'react';
import {
  Pressable,
  StyleSheet,
  Text,
  View,
} from 'react-native';

export default function CourseCard({ course, onPress }) {
  return (
    <Pressable style={styles.card} onPress={onPress}>
      <Text style={styles.title}>
        {course.titulo}
      </Text>

      <Text
        style={styles.description}
        numberOfLines={2}
      >
        {course.descricao || 'Sem descrição cadastrada.'}
      </Text>

      <View style={styles.infoRow}>
        <Text style={styles.infoText}>
          {course.cargaHoraria}h
        </Text>

        <Text style={styles.infoText}>
          {course.totalAulas} aulas
        </Text>
      </View>
    </Pressable>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: '#fff',
    padding: 16,
    borderRadius: 14,
    marginBottom: 12,
    elevation: 2,
  },

  title: {
    fontSize: 18,
    fontWeight: '700',
  },

  description: {
    color: '#666',
    marginTop: 6,
    lineHeight: 20,
  },

  infoRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginTop: 12,
  },

  infoText: {
    color: '#555',
  },
});