import Heading from './components/Heading';
import BowlerTable from './components/BowlerTable';
import './App.css';

function App() {
  return (
    <>
      {/* Page intro/header section */}
      <Heading />
      {/* Assignment table that loads and displays bowler data */}
      <BowlerTable />
    </>
  );
}

export default App;
