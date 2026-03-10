import { useEffect, useState } from 'react';

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
  const [bowlers, setBowlers] = useState<Bowler[]>([]);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadBowlers = async () => {
      try {
        const response = await fetch('http://localhost:5283/api/bowlers');

        if (!response.ok) {
          throw new Error(`Request failed: ${response.status}`);
        }

        const data: Bowler[] = await response.json();
        setBowlers(data);
      } catch (err) {
        console.error('Error loading bowlers:', err);
        setError('Could not load bowlers. Make sure the backend is running.');
      }
    };

    loadBowlers();
  }, []);

  if (error) {
    return <p>{error}</p>;
  }

  return (
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
          <tr key={`${b.teamName}-${b.bowlerLastName}-${b.bowlerFirstName}`}>
            <td>
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
