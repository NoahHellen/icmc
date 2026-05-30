import { borderRadius, colours, spacing } from '@styles/variables';
import { Modal, StyleSheet, View } from 'react-native';
import { Button } from './Button';
import { Body } from './Typography';

interface ConfirmToastProps {
  visible: boolean;
  message: string;
  onConfirm: () => void;
  onCancel: () => void;
}

export const ConfirmToast = ({
  visible,
  message,
  onConfirm,
  onCancel,
}: ConfirmToastProps) => {
  return (
    <Modal visible={visible} transparent animationType="fade">
      <View style={styles.overlay}>
        <View style={styles.container}>
          <Body style={styles.message}>{message}</Body>
          <View style={styles.actions}>
            <Button
              title="Confirm"
              onPress={onConfirm}
              variant="danger"
              size="small"
              style={styles.button}
            />
            <Button
              title="Cancel"
              onPress={onCancel}
              variant="ghost"
              size="small"
              style={styles.button}
            />
          </View>
        </View>
      </View>
    </Modal>
  );
};

const styles = StyleSheet.create({
  overlay: {
    flex: 1,
    backgroundColor: 'rgba(0, 0, 0, 0.6)',
    justifyContent: 'center',
    padding: spacing.large,
  },
  container: {
    backgroundColor: colours.surface,
    borderRadius: borderRadius.large,
    padding: spacing.large,
    alignItems: 'center',
    borderWidth: 1,
    borderColor: colours.whiteOpacityStrong,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 10 },
    shadowOpacity: 0.5,
    shadowRadius: 20,
    elevation: 10,
  },
  message: {
    color: '#fff',
    textAlign: 'center',
    marginBottom: spacing.large,
    fontSize: 16,
  },
  actions: {
    flexDirection: 'row',
    gap: spacing.medium,
    width: '100%',
  },
  button: {
    flex: 1,
  },
});
