import { UseComfortData } from '../hooks/useComfortData';
import { ComfortRankingList } from '../components/ComfortRankingList';

export function Dashboard() {
    const { data, loading, error, refetch } = UseComfortData();

    return (
        <div className="min-h-screen bg-gray-50 px-4 py-6 sm:px-6 lg:px-8">
            <div className="max-w-7xl mx-auto">
                <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6 gap-3">
                    <h1 className="text-2xl sm:text-3xl font-bold text-gray-900">
                        Comfort Index Dashboard
                    </h1>
                    <button
                        onClick={refetch}
                        className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors text-sm font-medium self-start sm:self-auto"
                    >
                        Refresh
                    </button>
                </div>

                {loading && (
                    <p className="text-gray-500 text-center py-12">Loading comfort data...</p>
                )}

                {error && (
                    <p className="text-red-600 text-center py-12">Error: {error}</p>
                )}

                {!loading && !error && data.length > 0 && (
                    <ComfortRankingList cities={data} />
                )}
            </div>
        </div>
    );
}