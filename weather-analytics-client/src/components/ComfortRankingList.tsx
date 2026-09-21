import { useState, useMemo } from 'react';
import type { CityComfortResult } from '../types/comfort';
import { CityCard } from './CityCard';

interface ComfortRankingListProps {
  cities: CityComfortResult[];
}

type SortOption = 'rank' | 'temp-asc' | 'temp-desc' | 'name';

export function ComfortRankingList({ cities }: ComfortRankingListProps) {
  const [sortBy, setSortBy] = useState<SortOption>('rank');
  const [filterText, setFilterText] = useState('');

  const filteredAndSorted = useMemo(() => {
    let result = cities.filter(city =>
      city.cityName.toLowerCase().includes(filterText.toLowerCase())
    );

    switch (sortBy) {
      case 'temp-asc':
        result = [...result].sort((a, b) => parseFloat(a.tempCelsius) - parseFloat(b.tempCelsius));
        break;
      case 'temp-desc':
        result = [...result].sort((a, b) => parseFloat(b.tempCelsius) - parseFloat(a.tempCelsius));
        break;
      case 'name':
        result = [...result].sort((a, b) => a.cityName.localeCompare(b.cityName));
        break;
      case 'rank':
      default:
        result = [...result].sort((a, b) => a.rank - b.rank);
        break;
    }

    return result;
  }, [cities, sortBy, filterText]);

  return (
    <div>
      <div className="flex flex-col sm:flex-row gap-3 mb-4">
        <input
          type="text"
          placeholder="Filter by city name..."
          value={filterText}
          onChange={(e) => setFilterText(e.target.value)}
          className="px-3 py-2 rounded-md border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 text-sm flex-1"
        />
        <select
          value={sortBy}
          onChange={(e) => setSortBy(e.target.value as SortOption)}
          className="px-3 py-2 rounded-md border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 text-sm"
        >
          <option value="rank">Sort: Comfort Rank</option>
          <option value="temp-asc">Sort: Temp (Low to High)</option>
          <option value="temp-desc">Sort: Temp (High to Low)</option>
          <option value="name">Sort: City Name</option>
        </select>
      </div>

      {filteredAndSorted.length === 0 ? (
        <p className="text-gray-500 dark:text-gray-400 text-center py-8">No cities match your filter.</p>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
          {filteredAndSorted.map((city) => (
            <CityCard key={city.cityName} city={city} />
          ))}
        </div>
      )}
    </div>
  );
}