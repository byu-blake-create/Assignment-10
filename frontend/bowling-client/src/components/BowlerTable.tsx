import { useEffect, useState } from 'react';

// Shape of each row returned by GET /api/bowlers.
type Bowler = {
  bowlerFirstName?: string;
  bowlerMiddleInit?: string;
  bowlerLastName?: string;
  teamName: string;
  bowlerAddress?: string;
  bowlerCity?: string;
  bowlerState?: string;
  bowlerZip?: string;
  bowlerPhoneNumber?: string;
};

function BowlerTable() {
  // Store API data for table rows.
  const [bowlers, setBowlers] = useState<Bowler[]>([]);
  // Store a user-friendly message if loading fails.
  const [error, setError] = useState('');

  useEffect(() => {
    // Load data once when this component is first rendered.
    const loadBowlers = async () => {
      try {
        // Call the backend endpoint for filtered bowlers.
        const response = await fetch('http://localhost:5283/api/bowlers');

        if (!response.ok) {
          throw new Error(`Request failed: ${response.status}`);
        }

        // Parse JSON and push it into component state.
        const data: Bowler[] = await response.json();
        setBowlers(data);
      } catch (err) {
        // Surface errors in UI and log full details for debugging.
        console.error('Error loading bowlers:', err);
        setError('Could not load bowlers. Make sure the backend is running.');
      }
    };

    loadBowlers();
  }, []);

  // Show a message instead of a blank table when data load fails.
  if (error) {
    return <p>{error}</p>;
  }

  return (
    // Render assignment-required columns for each bowler row.
    <table>
      <thead>
        <tr>
          <th>Name</th>
          <th>Team</th>
          <th>Address</th>
          <th>City</th>
          <th>State</th>
          <th>Zip</th>
          <th>Phone</th>
        </tr>
      </thead>
      <tbody>
        {bowlers.map((b) => (
          // Build a stable key from team + name values.
          <tr key={`${b.teamName}-${b.bowlerLastName}-${b.bowlerFirstName}`}>
            <td>
              {/* Merge first/middle/last into one clean display name */}
              {`${b.bowlerFirstName ?? ''} ${b.bowlerMiddleInit ?? ''} ${b.bowlerLastName ?? ''}`
                .replace(/\s+/g, ' ')
                .trim()}
            </td>
            <td>{b.teamName}</td>
            <td>{b.bowlerAddress}</td>
            <td>{b.bowlerCity}</td>
            <td>{b.bowlerState}</td>
            <td>{b.bowlerZip}</td>
            <td>{b.bowlerPhoneNumber}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

export default BowlerTable;
