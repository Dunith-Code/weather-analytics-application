import { useState, useEffect } from "react";
import type { CityComfortResult } from "../types/comfort";
import { useAuth0 } from "@auth0/auth0-react";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

interface UseComfortDataResult {
    data: CityComfortResult[];
    loading: boolean;
    error: string | null;
    refetch: () => void;
}

export function useComfortData(): UseComfortDataResult {
    const { getAccessTokenSilently, isAuthenticated } = useAuth0();
    const [data, setData] = useState<CityComfortResult[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [refetchTrigger, setRefetchTrigger] = useState<number>(0);

    useEffect(() => {
        if (!isAuthenticated) {
            setLoading(false);
            return;
        }

        const fetchData = async () => {
            setLoading(true);
            setError(null);

            try {
                const token = await getAccessTokenSilently();

                const response = await fetch(`${API_BASE_URL}/comfort/ranked-cities`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });

                if (!response.ok) {
                    throw new Error (`Server responded with status ${response.status}`);
                }

                const json: CityComfortResult[] = await response.json();
                setData(json);
            } catch (err) {
                setError(err instanceof Error ? err.message : 'Failed to fetch comfort data');
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [refetchTrigger, isAuthenticated, getAccessTokenSilently]);

    const refetch = () => setRefetchTrigger(prev => prev + 1);

    return { data, loading, error, refetch };
}