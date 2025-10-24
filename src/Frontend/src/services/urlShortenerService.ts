import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000';

export interface CreateShortUrlRequest {
    originalUrl: string;
}

export interface CreateShortUrlResponse {
    shortUrl: string;
    shortCode: string;
    originalUrl: string;
    createdAt: string;
}

export interface ErrorResponse {
    message: string;
    statusCode: number;
    timestamp: string;
    errors?: Record<string, string[]>;
}

/**
 * Service để gọi API tạo URL rút gọn
 */
class UrlShortenerService {
    /**
     * Tạo URL rút gọn
     */
    async createShortUrl(originalUrl: string): Promise<CreateShortUrlResponse> {
        try {
            const response = await axios.post<CreateShortUrlResponse>(
                `${API_BASE_URL}/api/shorten`,
                {
                    originalUrl
                } as CreateShortUrlRequest
            );

            return response.data;
        } catch (error: any) {
            if (axios.isAxiosError(error) && error.response) {
                const errorData = error.response.data as ErrorResponse;
                throw new Error(errorData.message || 'Đã xảy ra lỗi khi tạo URL rút gọn');
            }
            throw new Error('Không thể kết nối đến server');
        }
    }
}

export const urlShortenerService = new UrlShortenerService();
