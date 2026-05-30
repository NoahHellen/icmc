import { createContext, useContext, useState, useEffect } from 'react';
import type { UserDto } from '../api/users/usersTypes';
import { getToken, clearAuth } from '../utils/authStorage';
import { getMe } from '../api/users/usersService';

type UserContextType = {
  user: UserDto | null;
  setUser: (user: UserDto | null) => void;
  isInitialising: boolean;
  logout: () => Promise<void>;
};

const UserContext = createContext<UserContextType | null>(null);

export const UserProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<UserDto | null>(null);
  const [isInitialising, setIsInitialising] = useState(true);

  useEffect(() => {
    const initialiseAuth = async () => {
      try {
        const token = await getToken();

        if (token) {
          // Attempt to fetch current user details to verify token
          const userDetails = await getMe();
          setUser(userDetails);
        }
      } catch (error) {
        console.error('Failed to initialise auth', error);
        // On error (e.g. 401 Unauthorized), clear the invalid token
        await clearAuth();
      } finally {
        setIsInitialising(false);
      }
    };

    initialiseAuth();
  }, []);

  const logout = async () => {
    await clearAuth();
    setUser(null);
  };

  return (
    <UserContext.Provider value={{ user, setUser, isInitialising, logout }}>
      {children}
    </UserContext.Provider>
  );
};

export const useUserContext = () => {
  const context = useContext(UserContext);
  if (!context) {
    throw new Error('useUserContext must be used within UserProvider');
  }
  return context;
};
