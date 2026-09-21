import { useState, useEffect } from 'react';
import { useAuth0 } from '@auth0/auth0-react';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

interface ForecastPoint {
  time: string;
  tempCelsius: number;
}

export function useForecastData(cityCode: string | null) {
  const { getAccessTokenSilently } = useAuth0();
  const [forecast, setForecast] = useState<ForecastPoint[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!cityCode) return;

    const fetchForecast = async () => {
      setLoading(true);
      try {
        const token = await getAccessTokenSilently();
        const response = await fetch(`${API_BASE_URL}/forecast/${cityCode}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        const json: ForecastPoint[] = await response.json();
        setForecast(json);
      } catch {
        setForecast([]);
      } finally {
        setLoading(false);
      }
    };

    fetchForecast();
  }, [cityCode, getAccessTokenSilently]);

  return { forecast, loading };
}