import type { StorageLocation } from '@api/common/enums';
import Account from '@pages/Account';
import Camera from '@pages/Camera';
import GearItem from '@pages/GearItem';
import User from '@pages/User';
import Users from '@pages/Users';
import Login from '@pages/Login';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import TabRouter from './TabRouter';
import { useUserContext } from '@contexts/UserContext';
import * as SplashScreen from 'expo-splash-screen';
import { useEffect } from 'react';

export type RootStackParamList = {
  Login: undefined;
  Main: undefined;
  Account: undefined;
  GearItem: { id: number };
  User: { id: number };
  Camera: { id: number };
  // Kept for backward compatibility if navigation is called directly
  Loans: undefined;
  Home: undefined;
  Browse: { storageLocation?: StorageLocation } | undefined;
  Logbook: undefined;
  Users: undefined;
};

const Stack = createNativeStackNavigator<RootStackParamList>();

const BootRouter = () => {
  const { user, isInitialising } = useUserContext();

  useEffect(() => {
    if (!isInitialising) {
      SplashScreen.hideAsync();
    }
  }, [isInitialising]);

  if (isInitialising) {
    return null;
  }

  return (
    <Stack.Navigator
      screenOptions={{ headerShown: false }}
      initialRouteName={user ? "Main" : "Login"}
    >
      <Stack.Screen name="Login" component={Login} />
      <Stack.Screen name="Main" component={TabRouter} />
      <Stack.Screen name="Account" component={Account} />
      <Stack.Screen name="GearItem" component={GearItem} />
      <Stack.Screen name="User" component={User} />
      <Stack.Screen name="Camera" component={Camera} />
    </Stack.Navigator>
  );
};

export default BootRouter;
