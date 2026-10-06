import React from 'react';
import {
  View,
  Text,
  StyleSheet,
} from 'react-native';

export default function ProgressBar({ value }) {
  return (
    <View>
      <View style={styles.track}>
        <View
          style={[
            styles.fill,
            {
              width: `${value}%`,
            },
          ]}
        />
      </View>

      <Text style={styles.text}>
        {value}% concluído
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  track: {
    height: 9,
    borderRadius: 8,
    backgroundColor: '#e5e7eb',
    overflow: 'hidden',
  },

  fill: {
    height: '100%',
    backgroundColor: '#2563eb',
  },

  text: {
    fontSize: 12,
    color: '#555',
    marginTop: 5,
  },
});