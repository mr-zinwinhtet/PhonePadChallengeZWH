import { useState, useRef, useEffect } from 'react';
import './App.css';

// Adjustable timer for manual testing
const PAUSE_TIMEOUT_MS = 700; 

function App() {
  // What the user sees on the screen (no logical spaces)
  const [displaySequence, setDisplaySequence] = useState('');
  // What gets sent to the API (includes logical spaces for timeouts)
  const [backendSequence, setBackendSequence] = useState('');
  const [decodedText, setDecodedText] = useState('');
  
  // Use a ref to track the active timer so we can cancel it on new keystrokes
  const timerRef = useRef(null);

  // Cleanup timer if the component unmounts to prevent memory leaks
  useEffect(() => {
    return () => {
      if (timerRef.current) clearTimeout(timerRef.current);
    };
  }, []);

  const handlePress = async (char) => {
    // 1. Cancel any pending timeout because the user is actively typing
    if (timerRef.current) {
      clearTimeout(timerRef.current);
    }

    // 2. Handle the Send (#) Button
    if (char === '#') {
      const finalBackendSequence = backendSequence + '#';
      setDisplaySequence(prev => prev + '#');
      setBackendSequence(finalBackendSequence);

      try {
        const response = await fetch('http://127.0.0.1:5150/api/decode', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ input: finalBackendSequence })
        });

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        setDecodedText(data.output);
      } catch (error) {
        console.error('Error connecting to API:', error);
        setDecodedText(`Error: ${error.message}`);
      }
      return; // Exit early after sending
    }

    // 3. Update both sequences for standard typing
    setDisplaySequence(prev => prev + char);
    setBackendSequence(prev => prev + char);

    // 4. Start the pause timer (ignoring Backspace and 0/Space)
    if (char !== '*' && char !== '0') {
      timerRef.current = setTimeout(() => {
        setBackendSequence(prev => {
          // Only append a logical space if one hasn't just been added
          if (!prev.endsWith(' ')) {
            return prev + ' ';
          }
          return prev;
        });
      }, PAUSE_TIMEOUT_MS);
    }
  };

  const clearAll = () => {
    if (timerRef.current) clearTimeout(timerRef.current);
    setDisplaySequence('');
    setBackendSequence('');
    setDecodedText('');
  };

  const padButtons = [
    { label: '1', sub: '&nbsp;' },
    { label: '2', sub: 'ABC' },
    { label: '3', sub: 'DEF' },
    { label: '4', sub: 'GHI' },
    { label: '5', sub: 'JKL' },
    { label: '6', sub: 'MNO' },
    { label: '7', sub: 'PQRS' },
    { label: '8', sub: 'TUV' },
    { label: '9', sub: 'WXYZ' },
    { label: '*', sub: 'BACK' },
    { label: '0', sub: 'SPACE', value: '0' }, // Adjusted to send '0' to match backend mapping
    { label: '#', sub: 'SEND' }
  ];

  return (
    <div className="phone-container">
      <h2>Old Phone Pad Decoder</h2>
      
      <div className="screen">
        {/* We only render the displaySequence here so the user never sees the logical spaces */}
        <div className="input-sequence">Input: {displaySequence || '...'}</div>
        <div className="output-result">Result: <strong>{decodedText}</strong></div>
      </div>

      <div className="keypad">
        {padButtons.map((btn, index) => (
          <button 
            key={index} 
            className="key" 
            onClick={() => handlePress(btn.value !== undefined ? btn.value : btn.label)}
          >
            <span className="primary">{btn.label}</span>
            <span className="secondary" dangerouslySetInnerHTML={{ __html: btn.sub }}></span>
          </button>
        ))}
      </div>
      
      <button className="clear-btn" onClick={clearAll}>Clear Screen</button>
    </div>
  );
}

export default App;