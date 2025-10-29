import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import HomePage from './pages/HomePage';
import FullHistoryPage from './pages/FullHistoryPage';
import './App.css';

const App: React.FC = () => {
    return (
        <Router basename="/URLShortener">
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/history" element={<FullHistoryPage />} />
            </Routes>
        </Router>
    );
};

export default App;
