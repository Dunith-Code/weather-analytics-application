import type { CityComfortResult } from '../types/comfort';

interface CityCardProps {
    city: CityComfortResult;
}

function getScoreColor(score: number): string {
    if (score >= 85) return 'bg-green-100 text-green-800 border-green-300';
    if (score >= 70) return 'bg-yellow-100 text-yellow-800 border-yellow-300';
    return 'bg-red-100 text-red-800 border-red-300';
}

export function CityCard({ city }: CityCardProps) {
    return (
        <div className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm hover:shadow-md transition-shadow">
            <div className="flex items-start justify-between mb-2">
                <div>
                    <span className="text-xs font-semibold text-gray-400">#{city.rank}</span>
                    <h3 className="text-lg font-bold text-gray-900">{city.cityName}</h3>
                </div>
                <span className={`px-2 py-1 rounded-full text-sm font-semibold border ${getScoreColor(city.comfortScore)}`}>
                    {city.comfortScore.toFixed(1)}
                </span>
            </div>
            <p className="text-sm text-gray-600 capitalize mb-1">{city.weatherDescription}</p>
            <p className="text-2xl font-semibold text-gray-800">{parseFloat(city.tempCelsius).toFixed(1)}°C</p>
        </div>
    );
}