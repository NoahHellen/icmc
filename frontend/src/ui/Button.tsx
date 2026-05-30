import { borderRadius, colours, spacing } from '@styles/variables';
import {
  StyleSheet,
  TouchableOpacity,
  type TouchableOpacityProps,
} from 'react-native';
import { Body } from './Typography';

interface ButtonProps extends TouchableOpacityProps {
  title: string;
  variant?: 'primary' | 'secondary' | 'danger' | 'ghost';
  size?: 'small' | 'medium' | 'large';
}

export const Button = ({
  title,
  variant = 'primary',
  size = 'medium',
  style,
  ...props
}: ButtonProps) => {
  const getVariantStyle = () => {
    switch (variant) {
      case 'primary':
        return { 
          backgroundColor: colours.blue,
          borderWidth: 1,
          borderColor: 'rgba(255, 255, 255, 0.2)'
        };
      case 'secondary':
        return { 
          backgroundColor: 'transparent',
          borderWidth: 1,
          borderColor: colours.blue
        };
      case 'danger':
        return { 
          backgroundColor: 'transparent',
          borderWidth: 1,
          borderColor: colours.error
        };
      case 'ghost':
        return {
          backgroundColor: 'transparent',
          borderWidth: 1,
          borderColor: colours.whiteOpacityStrong,
        };
      default:
        return { backgroundColor: colours.blue };
    }
  };

  const getTextStyle = () => {
    switch (variant) {
      case 'secondary':
        return { color: colours.blue };
      case 'danger':
        return { color: colours.error };
      default:
        return { color: '#fff' };
    }
  };

  const getSizeStyle = () => {
    switch (size) {
      case 'small':
        return {
          paddingVertical: 6,
          paddingHorizontal: spacing.small,
        };
      case 'medium':
        return {
          paddingVertical: 10,
          paddingHorizontal: spacing.medium,
        };
      case 'large':
        return {
          paddingVertical: 14,
          paddingHorizontal: spacing.large,
        };
      default:
        return {
          paddingVertical: 10,
          paddingHorizontal: spacing.medium,
        };
    }
  };

  return (
    <TouchableOpacity
      style={[styles.button, getVariantStyle(), getSizeStyle(), style]}
      activeOpacity={0.7}
      {...props}
    >
      <Body style={[styles.text, getTextStyle()]}>{title}</Body>
    </TouchableOpacity>
  );
};

const styles = StyleSheet.create({
  button: {
    borderRadius: borderRadius.none, // Sharper modern look
    alignItems: 'center',
    justifyContent: 'center',
  },
  text: {
    fontWeight: '500',
    textTransform: 'uppercase',
    letterSpacing: 1.2,
    fontSize: 13,
  },
});
