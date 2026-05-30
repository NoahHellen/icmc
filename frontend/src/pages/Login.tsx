import { useCreateJwt, useSendOtp } from '@api/users/usersApi';
import { searchUsers } from '@api/users/usersService';
import { useUserContext } from '@contexts/UserContext';
import type { RootStackParamList } from '@navigation/BootRouter';
import { type NavigationProp, useNavigation } from '@react-navigation/native';
import { borderRadius, colours, spacing } from '@styles/variables';
import BackgroundComponent from '@ui/BackgroundComponent';
import { Card } from '@ui/Card';
import { Body, Heading } from '@ui/Typography';
import { saveToken } from '@utils/authStorage';
import { Key, LogIn, User } from 'lucide-react-native';
import { useState } from 'react';
import {
  ActivityIndicator,
  Alert,
  Keyboard,
  StyleSheet,
  TextInput,
  TouchableOpacity,
  TouchableWithoutFeedback,
  View,
} from 'react-native';

const Login = () => {
  const [cid, setCid] = useState<string>('');
  const [otp, setOtp] = useState<string>('');
  const [isOtpSent, setIsOtpSent] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { setUser } = useUserContext();
  const navigation = useNavigation<NavigationProp<RootStackParamList>>();

  const sendOtpMutation = useSendOtp();
  const createJwtMutation = useCreateJwt();

  const handleSendOtp = async () => {
    if (!cid.trim()) {
      setError('Please enter your CID');
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      await sendOtpMutation.mutateAsync({ Cid: cid });
      setIsOtpSent(true);
      Alert.alert(
        'Check Email',
        'A verification code has been sent to your email.'
      );
    } catch (err) {
      setError('Failed to send verification email. Please check your CID.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleLogin = async () => {
    if (!otp.trim()) {
      setError('Please enter the verification code');
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const token = await createJwtMutation.mutateAsync({ Cid: cid, Otp: otp });
      await saveToken(token);

      // Fetch user details to set in context
      const users = await searchUsers({ cid });
      if (users && users.length > 0) {
        setUser(users[0]);
        navigation.reset({
          index: 0,
          routes: [{ name: 'Main' }],
        });
      } else {
        setError('User not found after login');
      }
    } catch (err) {
      setError('Invalid verification code');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <TouchableWithoutFeedback onPress={Keyboard.dismiss} accessible={false}>
      <View style={{ flex: 1 }}>
        <BackgroundComponent>
          <View style={styles.container}>
            <View style={styles.header}>
              <Heading style={styles.title}>ICMC</Heading>
              <Body style={styles.subtitle}>Inventory Management</Body>
            </View>

            <Card style={styles.loginCard}>
              <View style={styles.inputContainer}>
                <User
                  size={20}
                  color={colours.textMuted}
                  style={styles.inputIcon}
                />
                <TextInput
                  style={styles.input}
                  placeholder="College ID (CID)"
                  placeholderTextColor={colours.textMuted}
                  onChangeText={setCid}
                  value={cid}
                  autoFocus={!isOtpSent}
                  autoCapitalize="none"
                  keyboardAppearance="dark"
                  editable={!isOtpSent}
                />
              </View>

              {isOtpSent && (
                <View style={styles.inputContainer}>
                  <Key
                    size={20}
                    color={colours.textMuted}
                    style={styles.inputIcon}
                  />
                  <TextInput
                    style={styles.input}
                    placeholder="Verification Code (OTP)"
                    placeholderTextColor={colours.textMuted}
                    onChangeText={setOtp}
                    value={otp}
                    autoFocus={isOtpSent}
                    autoCapitalize="none"
                    keyboardAppearance="dark"
                    keyboardType="number-pad"
                  />
                </View>
              )}

              {error && <Body style={styles.errorText}>{error}</Body>}

              <TouchableOpacity
                style={[styles.button, isLoading && styles.buttonDisabled]}
                onPress={isOtpSent ? handleLogin : handleSendOtp}
                disabled={isLoading}
              >
                {isLoading ? (
                  <ActivityIndicator color="#fff" />
                ) : (
                  <>
                    <LogIn size={20} color="#fff" />
                    <Body style={styles.buttonText}>
                      {isOtpSent ? 'Login' : 'Send verification email'}
                    </Body>
                  </>
                )}
              </TouchableOpacity>

              {isOtpSent && (
                <TouchableOpacity
                  onPress={() => setIsOtpSent(false)}
                  style={styles.backButton}
                  disabled={isLoading}
                >
                  <Body style={styles.backButtonText}>Use a different CID</Body>
                </TouchableOpacity>
              )}
            </Card>
          </View>
        </BackgroundComponent>
      </View>
    </TouchableWithoutFeedback>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: spacing.medium,
    justifyContent: 'center',
    alignItems: 'center',
  },
  header: {
    marginBottom: spacing.xxxLarge,
    alignItems: 'center',
  },
  title: {
    fontSize: 48,
    marginBottom: 0,
    color: colours.blue,
    fontWeight: '800',
  },
  subtitle: {
    letterSpacing: 4,
    textTransform: 'uppercase',
    fontSize: 10,
    color: colours.textMuted,
    marginTop: spacing.xxSmall,
  },
  loginCard: {
    width: '100%',
    padding: spacing.large,
  },
  inputContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: colours.surfaceLight,
    borderRadius: borderRadius.medium,
    paddingHorizontal: spacing.medium,
    marginBottom: spacing.large,
    borderWidth: 1,
    borderColor: colours.whiteOpacity,
  },
  inputIcon: {
    marginRight: spacing.small,
  },
  input: {
    flex: 1,
    height: 50,
    color: colours.textPrimary,
    fontSize: 16,
  },
  button: {
    width: '100%',
    height: 50,
    backgroundColor: colours.purple,
    borderRadius: borderRadius.medium,
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    gap: spacing.small,
  },
  buttonDisabled: {
    opacity: 0.6,
  },
  buttonText: {
    color: '#fff',
    fontWeight: '700',
  },
  errorText: {
    color: colours.error,
    marginBottom: spacing.medium,
    textAlign: 'center',
    fontSize: 14,
  },
  backButton: {
    marginTop: spacing.medium,
    alignItems: 'center',
  },
  backButtonText: {
    color: colours.textMuted,
    fontSize: 14,
  },
});

export default Login;
