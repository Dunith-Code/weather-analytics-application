import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { useForecastData } from '../hooks/useForecastData';

interface TemperatureTrendChartProps {
  cityCode: string;
  cityName: string;
}

export function TemperatureTrendChart({ cityCode, cityName }: TemperatureTrendChartProps) {
  const { forecast, loading } = useForecastData(cityCode);

  if (loading) return <p className="text-sm text-gray-500 dark:text-gray-400">Loading trend...</p>;
  if (forecast.length === 0) return null;

  const chartData = forecast.map(point => ({
    time: point.time.split(' ')[1]?.slice(0, 5) ?? point.time,
    temp: point.tempCelsius,
  }));

  return (
    <div className="mt-2">
      <p className="text-xs font-semibold text-gray-500 dark:text-gray-400 mb-1">{cityName} — Next 24h</p>
      <ResponsiveContainer width="100%" height={100}>
        <LineChart data={chartData}>
          <CartesianGrid strokeDasharray="3 3" opacity={0.2} />
          <XAxis dataKey="time" tick={{ fontSize: 10 }} />
          <YAxis tick={{ fontSize: 10 }} width={30} />
          <Tooltip />
          <Line type="monotone" dataKey="temp" stroke="#3b82f6" strokeWidth={2} dot={false} />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
}