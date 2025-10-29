import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { urlShortenerService } from '../services/urlShortenerService';
import { urlHistoryService } from '../services/urlHistoryService';
import HistoryList from '../components/HistoryList';
import './HomePage.css';

interface HomePageState {
    originalUrl: string;
    shortUrl: string;
    loading: boolean;
    error: string;
    success: boolean;
}

const HomePage: React.FC = () => {
    const navigate = useNavigate();
    const [state, setState] = useState<HomePageState>({
        originalUrl: '',
        shortUrl: '',
        loading: false,
        error: '',
        success: false
    });

    const [historyUpdateTrigger, setHistoryUpdateTrigger] = useState(0);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setState((prev: any) => ({
            ...prev,
            originalUrl: e.target.value,
            error: '',
            success: false
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!state.originalUrl.trim()) {
            setState((prev: any) => ({ ...prev, error: 'Vui lòng nhập URL' }));
            return;
        }

        try {
            new URL(state.originalUrl);
        } catch {
            setState((prev: any) => ({ ...prev, error: 'URL không hợp lệ' }));
            return;
        }

        setState((prev: any) => ({ ...prev, loading: true, error: '', success: false }));

        try {
            const response = await urlShortenerService.createShortUrl(state.originalUrl);

            urlHistoryService.addToHistory({
                id: Date.now().toString(),
                originalUrl: response.originalUrl,
                shortUrl: response.shortUrl,
                shortCode: response.shortCode,
                createdAt: response.createdAt
            });

            setHistoryUpdateTrigger(prev => prev + 1);

            setState((prev: any) => ({
                ...prev,
                shortUrl: response.shortUrl,
                loading: false,
                success: true,
                error: ''
            }));
        } catch (error: any) {
            setState((prev: any) => ({
                ...prev,
                loading: false,
                error: error.message || 'Đã xảy ra lỗi khi tạo URL rút gọn',
                success: false
            }));
        }
    };

    const handleCopy = async () => {
        try {
            await navigator.clipboard.writeText(state.shortUrl);
            alert('Đã sao chép URL vào clipboard!');
        } catch (error) {
            alert('Không thể sao chép URL');
        }
    };

    const handleReset = () => {
        setState({
            originalUrl: '',
            shortUrl: '',
            loading: false,
            error: '',
            success: false
        });
    };

    return (
        <div className="home-page">
            <div className="content-wrapper">
                <header className="app-header">
                    <h1>🔗 URL Shortener</h1>
                    <p className="subtitle">Rút gọn link nhanh chóng và đơn giản</p>
                </header>

                <div className="main-content">
                    <form onSubmit={handleSubmit} className="url-form">
                        <div className="input-group">
                            <input
                                type="text"
                                value={state.originalUrl}
                                onChange={handleInputChange}
                                placeholder="Nhập URL cần rút gọn (vd: https://example.com)"
                                className="url-input"
                                disabled={state.loading}
                            />
                        </div>

                        <button
                            type="submit"
                            className="submit-button"
                            disabled={state.loading || !state.originalUrl.trim()}
                        >
                            {state.loading ? 'Đang xử lý...' : 'Rút gọn'}
                        </button>
                    </form>

                    {state.error && (
                        <div className="error-message">
                            <span className="error-icon">⚠️</span>
                            <span>{state.error}</span>
                        </div>
                    )}

                    {state.success && state.shortUrl && (
                        <div className="success-container">
                            <div className="success-message">
                                <span className="success-icon">✅</span>
                                <span>URL đã được rút gọn thành công!</span>
                            </div>

                            <div className="result-box">
                                <div className="result-label">URL rút gọn:</div>
                                <div className="result-url">
                                    <a
                                        href={state.shortUrl}
                                        target="_blank"
                                        rel="noopener noreferrer"
                                        className="short-url-link"
                                    >
                                        {state.shortUrl}
                                    </a>
                                </div>

                                <div className="result-actions">
                                    <button onClick={handleCopy} className="copy-button">
                                        📋 Sao chép
                                    </button>
                                    <button onClick={handleReset} className="reset-button">
                                        🔄 Tạo mới
                                    </button>
                                </div>
                            </div>
                        </div>
                    )}
                </div>

                {/* Hiển thị 2 link mới nhất */}
                <HistoryList
                    key={historyUpdateTrigger}
                    limit={2}
                    onViewAll={() => navigate('/history')}
                />

                <footer className="app-footer">
                    <p>© 2025 URL Shortener - Microservice Architecture</p>
                </footer>
            </div>
        </div>
    );
};

export default HomePage;
