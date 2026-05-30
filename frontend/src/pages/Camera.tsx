import { useUploadGearItemImage } from '@api/gear-items/gearItemsApi';
import { useUserContext } from '@contexts/UserContext';
import type { RootStackParamList } from '@navigation/BootRouter';
import {
  type RouteProp,
  useNavigation,
  useRoute,
} from '@react-navigation/native';
import { spacing } from '@styles/variables';
import BackgroundComponent from '@ui/BackgroundComponent';
import { Button } from '@ui/Button';
import { Heading } from '@ui/Typography';
import { CameraView, useCameraPermissions } from 'expo-camera';
import { useRef, useState } from 'react';
import {
  ActivityIndicator,
  Dimensions,
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
} from 'react-native';

const { width: SCREEN_WIDTH, height: SCREEN_HEIGHT } = Dimensions.get('window');
const BOX_WIDTH = SCREEN_WIDTH - spacing.medium * 2;
const BOX_HEIGHT = 300;
const BOX_TOP = (SCREEN_HEIGHT - BOX_HEIGHT) / 2;

const CameraPage = () => {
  const [permission, requestPermission] = useCameraPermissions();
  const navigation = useNavigation();
  const route = useRoute<RouteProp<RootStackParamList, 'Camera'>>();
  const { id } = route.params;
  const { user } = useUserContext();
  const cameraRef = useRef<CameraView>(null);
  const [isCapturing, setIsCapturing] = useState(false);
  const { mutateAsync: uploadImage } = useUploadGearItemImage();

  if (!user?.isAdmin) {
    return (
      <BackgroundComponent>
        <View style={styles.container}>
          <View style={styles.permissionContent}>
            <Heading
              style={{ textAlign: 'center', marginBottom: spacing.medium }}
            >
              Only admins can take photos.
            </Heading>
            <Button
              onPress={() => navigation.goBack()}
              title="Go Back"
              variant="secondary"
            />
          </View>
        </View>
      </BackgroundComponent>
    );
  }

  if (!permission) {
    // Camera permissions are still loading.
    return <View />;
  }

  if (!permission.granted) {
    // Camera permissions are not granted yet.
    return (
      <BackgroundComponent>
        <View style={styles.container}>
          <View style={styles.permissionContent}>
            <Heading
              style={{ textAlign: 'center', marginBottom: spacing.medium }}
            >
              We need your permission to show the camera
            </Heading>
            <Button onPress={requestPermission} title="Grant Permission" />
            <View style={{ height: spacing.medium }} />
            <Button
              onPress={() => navigation.goBack()}
              title="Go Back"
              variant="secondary"
            />
          </View>
        </View>
      </BackgroundComponent>
    );
  }

  const takePicture = async () => {
    if (cameraRef.current && !isCapturing) {
      try {
        setIsCapturing(true);
        const photo = await cameraRef.current.takePictureAsync({
          quality: 0.7,
          base64: false,
        });

        if (photo) {
          // Prepare the image data for the API
          const filename = photo.uri.split('/').pop();
          const match = /\.(\w+)$/.exec(filename || '');
          const type = match ? `image/${match[1]}` : 'image';

          const imageData = {
            uri: photo.uri,
            name: filename,
            type,
          } as any;

          await uploadImage({
            id,
            request: {
              id,
              imageData: imageData,
            },
          });

          navigation.goBack();
        }
      } catch (error) {
        console.error('Failed to take or upload picture:', error);
        alert('Failed to save the image. Please try again.');
      } finally {
        setIsCapturing(false);
      }
    }
  };

  return (
    <View style={styles.cameraContainer}>
      <CameraView style={styles.camera} facing="back" ref={cameraRef}>
        <View style={styles.overlay}>
          {/* Dimmed Background Overlays */}
          <View
            style={[
              styles.dimmedOverlay,
              { top: 0, left: 0, right: 0, height: BOX_TOP },
            ]}
          />
          <View
            style={[
              styles.dimmedOverlay,
              { top: BOX_TOP + BOX_HEIGHT, left: 0, right: 0, bottom: 0 },
            ]}
          />
          <View
            style={[
              styles.dimmedOverlay,
              {
                top: BOX_TOP,
                left: 0,
                width: spacing.medium,
                height: BOX_HEIGHT,
              },
            ]}
          />
          <View
            style={[
              styles.dimmedOverlay,
              {
                top: BOX_TOP,
                right: 0,
                width: spacing.medium,
                height: BOX_HEIGHT,
              },
            ]}
          />

          {/* Bounding Box */}
          <View style={styles.boundingBox}>
            <View style={styles.cornerTopLeft} />
            <View style={styles.cornerTopRight} />
            <View style={styles.cornerBottomLeft} />
            <View style={styles.cornerBottomRight} />
          </View>

          <View style={styles.uiContainer}>
            <TouchableOpacity
              style={styles.backButton}
              onPress={() => navigation.goBack()}
            >
              <Text style={styles.text}>Back</Text>
            </TouchableOpacity>

            <View style={styles.captureContainer}>
              <TouchableOpacity
                style={styles.captureButton}
                onPress={takePicture}
                disabled={isCapturing}
              >
                {isCapturing ? (
                  <ActivityIndicator color="#fff" />
                ) : (
                  <View style={styles.captureButtonInner} />
                )}
              </TouchableOpacity>
            </View>
          </View>
        </View>
      </CameraView>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    padding: spacing.large,
  },
  permissionContent: {
    alignItems: 'center',
  },
  cameraContainer: {
    flex: 1,
    backgroundColor: '#000',
  },
  camera: {
    flex: 1,
  },
  overlay: {
    flex: 1,
    backgroundColor: 'transparent',
  },
  dimmedOverlay: {
    position: 'absolute',
    backgroundColor: 'rgba(0,0,0,0.5)',
  },
  boundingBox: {
    position: 'absolute',
    top: BOX_TOP,
    left: spacing.medium,
    width: BOX_WIDTH,
    height: BOX_HEIGHT,
    borderWidth: 1,
    borderColor: 'rgba(255, 255, 255, 0.5)',
    borderRadius: spacing.small,
  },
  cornerTopLeft: {
    position: 'absolute',
    top: -1,
    left: -1,
    width: 20,
    height: 20,
    borderTopWidth: 3,
    borderLeftWidth: 3,
    borderColor: '#fff',
    borderTopLeftRadius: spacing.small,
  },
  cornerTopRight: {
    position: 'absolute',
    top: -1,
    right: -1,
    width: 20,
    height: 20,
    borderTopWidth: 3,
    borderRightWidth: 3,
    borderColor: '#fff',
    borderTopRightRadius: spacing.small,
  },
  cornerBottomLeft: {
    position: 'absolute',
    bottom: -1,
    left: -1,
    width: 20,
    height: 20,
    borderBottomWidth: 3,
    borderLeftWidth: 3,
    borderColor: '#fff',
    borderBottomLeftRadius: spacing.small,
  },
  cornerBottomRight: {
    position: 'absolute',
    bottom: -1,
    right: -1,
    width: 20,
    height: 20,
    borderBottomWidth: 3,
    borderRightWidth: 3,
    borderColor: '#fff',
    borderBottomRightRadius: spacing.small,
  },
  uiContainer: {
    flex: 1,
    justifyContent: 'space-between',
    padding: spacing.large,
  },
  backButton: {
    alignSelf: 'flex-start',
    alignItems: 'center',
    backgroundColor: 'rgba(0,0,0,0.5)',
    padding: spacing.small,
    borderRadius: spacing.xSmall,
    minWidth: 80,
    marginTop: spacing.large,
  },
  captureContainer: {
    alignItems: 'center',
    marginBottom: spacing.xLarge,
  },
  captureButton: {
    width: 70,
    height: 70,
    borderRadius: 35,
    backgroundColor: 'rgba(255, 255, 255, 0.3)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  captureButtonInner: {
    width: 60,
    height: 60,
    borderRadius: 30,
    backgroundColor: '#fff',
  },
  text: {
    fontSize: 18,
    fontWeight: 'bold',
    color: 'white',
  },
});

export default CameraPage;
