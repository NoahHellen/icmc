import queryClient from '@api/queryClient';
import {
  IBMPlexMono_300Light,
  IBMPlexMono_400Regular,
  IBMPlexMono_500Medium,
  IBMPlexMono_700Bold,
  useFonts,
} from '@expo-google-fonts/ibm-plex-mono';
import BootRouter from '@navigation/BootRouter';
import { DefaultTheme, NavigationContainer } from '@react-navigation/native';
import { QueryClientProvider } from '@tanstack/react-query';
import BackgroundComponent from '@ui/BackgroundComponent';
import * as SplashScreen from 'expo-splash-screen';
import { useCallback } from 'react';
import { StyleSheet, View } from 'react-native';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { UserProvider } from './src/contexts/UserContext';

SplashScreen.preventAutoHideAsync();

export default function App() {
  const [fontsLoaded, fontError] = useFonts({
    IBMPlexMono_300Light,
    IBMPlexMono_400Regular,
    IBMPlexMono_500Medium,
    IBMPlexMono_700Bold,
  });

  const onLayoutRootView = useCallback(async () => {
    // Splash screen is hidden in BootRouter once fonts and auth are ready
  }, []);

  if (!fontsLoaded && !fontError) {
    return null;
  }

  const NavTheme = {
    ...DefaultTheme,
    colors: {
      ...DefaultTheme.colors,
      background: 'transparent',
    },
  };

  return (
    <SafeAreaProvider onLayout={onLayoutRootView}>
      <QueryClientProvider client={queryClient}>
        <UserProvider>
          <View style={styles.backgroundLayer}>
            <BackgroundComponent />
          </View>
          <View style={styles.appLayer}>
            <NavigationContainer theme={NavTheme}>
              <BootRouter />
            </NavigationContainer>
          </View>
        </UserProvider>
      </QueryClientProvider>
    </SafeAreaProvider>
  );
}

const styles = StyleSheet.create({
  backgroundLayer: {
    position: 'absolute',
    top: 0,
    bottom: 0,
    left: 0,
    right: 0,
  },
  appLayer: {
    flex: 1,
    backgroundColor: 'transparent',
  },
});
