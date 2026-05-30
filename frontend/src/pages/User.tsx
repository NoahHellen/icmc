import { useGetUser } from '@api/users/usersApi';
import type { RootStackParamList } from '@navigation/BootRouter';
import {
  type NavigationProp,
  type RouteProp,
  useNavigation,
  useRoute,
} from '@react-navigation/native';
import { colours, spacing } from '@styles/variables';
import BackgroundComponent from '@ui/BackgroundComponent';
import { Badge } from '@ui/Badge';
import { Card } from '@ui/Card';
import HeaderComponent from '@ui/HeaderComponent';
import { InfoRow } from '@ui/InfoRow';
import { Body, Heading, Subheading } from '@ui/Typography';
import { getMemberTypeLabel } from '@utils/enumHelpers';
import { User as UserIcon } from 'lucide-react-native';
import { ScrollView, StyleSheet, View } from 'react-native';

const UserPage = () => {
  const navigation = useNavigation<NavigationProp<RootStackParamList>>();
  const route = useRoute<RouteProp<RootStackParamList, 'User'>>();
  const { id } = route.params;
  const { data: user, isLoading } = useGetUser(id);

  if (isLoading) {
    return (
      <BackgroundComponent>
        <View style={styles.center}>
          <Body>Loading...</Body>
        </View>
      </BackgroundComponent>
    );
  }

  if (!user) {
    return (
      <BackgroundComponent>
        <View style={styles.center}>
          <Body>User not found.</Body>
        </View>
      </BackgroundComponent>
    );
  }

  return (
    <BackgroundComponent>
      <HeaderComponent />
      <ScrollView contentContainerStyle={styles.container}>
        <View style={styles.header}>
          <View style={styles.avatarContainer}>
            <UserIcon size={64} color={colours.blue} />
          </View>
          <Heading style={styles.title}>{user.fullName || 'No Name'}</Heading>
          <Subheading style={styles.subtitle}>
            {user.email || 'No Email'}
          </Subheading>
          <View style={styles.badges}>
            <Badge
              label={getMemberTypeLabel(user.memberType)}
              backgroundColor={colours.blue}
              color="#fff"
            />
            {user.isAdmin && (
              <Badge
                label="ADMIN"
                backgroundColor={colours.purple}
                color="#fff"
              />
            )}
          </View>
        </View>

        <Card style={styles.section}>
          <View style={styles.sectionHeader}>
            <Subheading style={styles.sectionTitle}>Account Info</Subheading>
          </View>
          <InfoRow label="User ID" value={user.id.toString()} />
          <InfoRow label="Cid" value={user.cid || 'N/A'} last />
        </Card>
      </ScrollView>
    </BackgroundComponent>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: spacing.medium,
  },
  center: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
  header: {
    marginBottom: spacing.large,
    alignItems: 'center',
  },
  avatarContainer: {
    width: 100,
    height: 100,
    borderRadius: 50,
    backgroundColor: `${colours.blue}20`,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: spacing.medium,
  },
  title: {
    color: '#fff',
    marginBottom: 0,
    textAlign: 'center',
    fontSize: 22,
  },
  subtitle: {
    color: '#fff',
    opacity: 0.8,
    textAlign: 'center',
    marginBottom: spacing.small,
    fontSize: 10,
  },
  badges: {
    flexDirection: 'row',
    gap: spacing.small,
    marginTop: spacing.small,
  },
  section: {
    marginBottom: spacing.medium,
  },
  sectionHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: spacing.medium,
    borderBottomWidth: 1,
    borderBottomColor: 'rgba(255, 255, 255, 0.2)',
  },
  sectionTitle: {
    color: '#fff',
    fontSize: 16,
    paddingBottom: spacing.xSmall,
    marginBottom: 0,
  },
});

export default UserPage;
