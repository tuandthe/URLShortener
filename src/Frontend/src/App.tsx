import React, { useState } from 'react';
import { urlShortenerService } from './services/urlShortenerService';
import './App.css';

interface AppState {
    originalUrl: string;
    shortUrl: string;
    loading: boolean;
    error: string;
    success: boolean;
}

const App: React.FC = () => {
    const [state, setState] = useState<AppState>({
        originalUrl: '',
        shortUrl: '',
        loading: false,
        error: '',
        success: false
    });

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

        // Validation
        if (!state.originalUrl.trim()) {
            setState((prev: any) => ({ ...prev, error: 'Vui lòng nhập URL' }));
            return;
        }

        try {
            // Validate URL format
            new URL(state.originalUrl);
        } catch {
            setState((prev: any) => ({ ...prev, error: 'URL không hợp lệ' }));
            return;
        }

        setState((prev: any) => ({ ...prev, loading: true, error: '', success: false }));

        try {
            const response = await urlShortenerService.createShortUrl(state.originalUrl);

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
        <div className="app-container">
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

                                <div className="action-buttons">
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

                <footer className="app-footer">
                    <p>© 2025 URL Shortener - Microservice Architecture</p>
                </footer>
            </div>
        </div>
    );
};

export default App;
