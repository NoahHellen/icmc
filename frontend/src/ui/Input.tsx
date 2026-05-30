import { borderRadius, spacing, fonts } from '@styles/variables';
import { StyleSheet, TextInput, type TextInputProps, View } from 'react-native';
import { Label } from './Typography';

interface InputProps extends TextInputProps {
  label?: string;
}

export const Input = ({ label, style, ...props }: InputProps) => {
  return (
    <View style={styles.container}>
      {label && <Label style={styles.label}>{label}</Label>}
      <TextInput
        style={[styles.input, style]}
        placeholderTextColor="rgba(255, 255, 255, 0.3)"
        {...props}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    marginBottom: spacing.medium,
    width: '100%',
  },
  label: {
    color: 'rgba(255, 255, 255, 0.6)',
    marginBottom: 6,
    fontSize: 11,
    textTransform: 'uppercase',
    letterSpacing: 1,
  },
  input: {
    backgroundColor: 'rgba(255, 255, 255, 0.03)',
    borderRadius: borderRadius.none,
    paddingHorizontal: spacing.medium,
    paddingVertical: 12,
    color: '#fff',
    fontSize: 14,
    fontFamily: fonts.regular,
    borderWidth: 1,
    borderColor: 'rgba(255, 255, 255, 0.15)',
  },
});
