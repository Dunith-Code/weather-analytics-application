import { useAuth0 } from '@auth0/auth0-react';

export function LogoutButton() {
    const { logout } = useAuth0();

    return (
        <button
            onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}
            className="px-4 py-2 bg-gray-200 text-gray-800 rounded-md hover:bg-gray-300 transition-colors text-sm font-medium"
        >
            Logout
        </button>
    );
}