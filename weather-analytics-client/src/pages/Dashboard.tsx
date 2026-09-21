import { useAuth0 } from '@auth0/auth0-react';
import { useComfortData } from '../hooks/useComfortData';
import { ComfortRankingList } from '../components/ComfortRankingList';
import { LoginButton } from '../components/LoginButton';
import { LogoutButton } from '../components/LogoutButton';
import { DarkModeToggle } from '../components/DarkModeToggle';

export function Dashboard() {
    const { isAuthenticated, isLoading: authLoading, user } = useAuth0();
    const { data, loading, error, refetch } = useComfortData();

    if (authLoading) {
        return <p className="text-center py-12 text-gray-500 dark:text-gray-400">Loading...</p>;
    }

    if (!isAuthenticated) {
        return (
            <div className="min-h-screen flex flex-col items-center justify-center gap-4 bg-gray-50 dark:bg-gray-900">
                <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Comfort Index Dashboard</h1>
                <p className="text-gray-600 dark:text-gray-400">Please log in to view weather comfort rankings.</p>
                <LoginButton />
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-900 px-4 py-6 sm:px-6 lg:px-8 transition-colors">
            <div className="max-w-7xl mx-auto">
                <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6 gap-3">
                    <div>
                        <h1 className="text-2xl sm:text-3xl font-bold text-gray-900 dark:text-gray-100">
                            Comfort Index Dashboard
                        </h1>
                        <p className="text-sm text-gray-500 dark:text-gray-400">Logged in as {user?.email}</p>
                    </div>
                    <div className="flex gap-2 self-start sm:self-auto">
                        <DarkModeToggle />
                        <button
                            onClick={refetch}
                            className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors text-sm font-medium"
                        >
                            Refresh
                        </button>
                        <LogoutButton />
                    </div>
                </div>

                {loading && (
                    <p className="text-gray-500 dark:text-gray-400 text-center py-12">Loading comfort data...</p>
                )}

                {error && (
                    <p className="text-red-600 dark:text-red-400 text-center py-12">Error: {error}</p>
                )}

                {!loading && !error && data.length > 0 && (
                    <ComfortRankingList cities={data} />
                )}
            </div>
        </div>
    );
}