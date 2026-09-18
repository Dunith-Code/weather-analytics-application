import type { CityComfortResult } from '../types/comfort';
import { CityCard } from './CityCard';

interface ComfortRankingListProps {
    cities: CityComfortResult[];
}

export function ComfortRankingList({ cities }: ComfortRankingListProps) {
    return (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
            {cities.map((city) => (
                <CityCard key={city.cityName} city={city} />
            ))}
        </div>
    );
}